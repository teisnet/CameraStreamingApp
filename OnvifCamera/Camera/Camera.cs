using System;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace OnvifCamera
{
	/*
	 * https://github.com/aspnet/JavaScriptServices/tree/master/src/Microsoft.AspNetCore.NodeServices
	 */


	public class Camera : ICamera
	{

		private INodeServices nodeService;

		private string nodeFilename = "./node-camera.js";


		// Timers
		private Timer heartbeatTimer = new Timer(7000);
		private Timer statusTimer = new Timer(100);

		// Camera state fields
		private bool isInitialized = false;

		private bool isOnline = false;
		private bool isEnabled = false;

		private bool pendingStatus = false;
		private bool pendingConnect = false;

		private bool isMoving = false;
		private bool isMovingToTarget;

		private CameraPosition moveTarget;
		private CameraPosition position;
		private CameraPosition previousPosition;

		private JToken nodeOnvifCamera;

		private readonly ILogger<Camera> logger;

		public dynamic Capabilities { get; set; }
		public dynamic VideoSources { get; set; }
		public dynamic Profiles { get; set; }
		public dynamic DefaultProfile { get; set; }
		public dynamic ActiveSource { get; set; }

		public string Name => config.Name;
		public string Uri => config.Uri; // UriBuilder?

		public PtzRange Range;

		// Events
		public event EventHandler Moving;
		public event EventHandler StatusChanged;

		CameraConfig config;
		private int configHash;


		public Camera() { }

		// The dependency injection container will automatically use this constructor.
		public Camera(IOptionsMonitor<CameraConfig> config, ILogger<Camera> logger, INodeServices nodeServices)
		{
			this.logger = logger;
			this.config = config.CurrentValue;
			this.configHash = this.config.GetHashCode();

			// This event is fired when the camera settings in appsetting.json are changed.
			config.OnChange(config =>
			{
				// For some reason OnChange is fired twice per change. Don't act if the config properties haven't changed.
				// See https://github.com/dotnet/aspnetcore/issues/2542
				var newConfigHash = config.GetHashCode();

				if (newConfigHash != configHash)
				{
					this.config = config;
					configHash = newConfigHash;
					logger.LogInformation("The camera configuration has been updated.");

					OnConfigChange();
				}
			});


			this.nodeService = nodeServices;

			this.heartbeatTimer.Elapsed += (sender, e) => this.Connect();
			this.statusTimer.Elapsed += (sender, e) => this.UpdateStatus();
		}

		private void OnConfigChange()
		{
		}

		private async Task<bool> Init()
		{
			if (this.isInitialized) return true;

			if (nodeOnvifCamera == null)
			{
				// Create new node instance
				// TODO: Handle failed initialization, eg. no when there is connection to the camera
				nodeOnvifCamera = await Call<JToken>("init", config.Uri, config.OnvifPort, config.Username, config.Password);
			}
			else
			{
				// Update existing instance. Only meaningful if parameters have been changed, eg. the hostname
				nodeOnvifCamera = await Call<JToken>("connect");
			}

			Capabilities = nodeOnvifCamera["capabilities"].ToObject<dynamic>();
			VideoSources = nodeOnvifCamera["videoSources"].ToObject<dynamic>();
			Profiles = nodeOnvifCamera["profiles"].ToObject<dynamic>();
			DefaultProfile = nodeOnvifCamera["defaultProfile"].ToObject<dynamic>();
			ActiveSource = nodeOnvifCamera["activeSource"].ToObject<dynamic>();

			dynamic ptzConfiguration = this.DefaultProfile.PTZConfiguration;


			dynamic zoomLimits = ptzConfiguration.zoomLimits.range.XRange;
			float zoomMax = zoomLimits.max.ToObject<float>();
			float zoomMin = zoomLimits.min.ToObject<float>();

			dynamic panTiltLimits = ptzConfiguration.panTiltLimits.range;
			float xMin = panTiltLimits.XRange.min.ToObject<float>();
			float xMax = panTiltLimits.XRange.max.ToObject<float>();
			float yMin = panTiltLimits.YRange.min.ToObject<float>();
			float yMax = panTiltLimits.YRange.max.ToObject<float>();

			this.Range = new PtzRange()
			{
				X = new Range<float>(xMin, xMax),
				Y = new Range<float>(yMin, yMax),
				Zoom = new Range<float>(zoomMin, zoomMax)
			};

			this.moveTarget = new CameraPosition(this.Range);
			this.position = new CameraPosition(this.Range);
			this.previousPosition = new CameraPosition(this.Range);

			this.isInitialized = true;

			return this.isInitialized;
		}

		public async Task Enable()
		{
			if (this.isEnabled) return;
			this.isEnabled = true;

			logger?.LogInformation("Enabled");

			// In case camera is not online, emit 'enabled' at least.
			OnStatusChanged(/*isEnabled*/);

			await this.Connect();
			this.heartbeatTimer.Start();
		}

		public void Disable()
		{
			if (!this.isEnabled) return;
			this.isEnabled = false;
			// TODO: Stop any movements in progress
			this.heartbeatTimer.Stop();
			if (this.isOnline)
			{
				this.SetOnline(false);
			}
			else
			{
				OnStatusChanged(/*isEnabled*/);
			}

			logger?.LogInformation("Disabled");
		}

		private async Task Connect()
		{
			if (this.pendingConnect || this.pendingStatus) return;

			if (!this.isInitialized)
			{
				try
				{
					this.pendingConnect = true;
					await Init();
				}
				catch (Exception e)
				{
					logger?.LogError(e, "Could not connect to camera using ONVIF");
					return;
				}
				finally
				{
					this.pendingConnect = false;
				}
			}

			this.SetOnline(true);

			await this.UpdateStatus();
		}

		private void SetOnline(bool value)
		{
			if (this.isOnline == value) return;
			this.isOnline = value;

			logger?.LogInformation(this.isOnline ? "Connected" : "Disconnected");

			OnStatusChanged(/*isOnline*/);
		}

		// Consider case when camera hasn't startet moving yet. Consider 'settle' period.
		private async Task UpdateStatus()
		{
			if (this.pendingStatus || !this.isInitialized) return;

			// TODO: Consider replacement by thread 'lock'

			PtzValue statusPosition;

			try
			{
				this.pendingStatus = true;
				statusPosition = await Call<PtzValue>("getStatus");
			}
			catch (Exception e)
			{
				logger.LogError(e, "Could not update status");
				// TODO: Check exception message
				this.SetOnline(false);
				return;
			}
			finally
			{
				this.pendingStatus = false;
			}

			this.previousPosition = this.position;

			this.position.Native = statusPosition;

			// Check if the camera is still moving
			if (this.previousPosition != this.position)
			{
				// The camera is still moving

				this.isMoving = true;

				// Increase update interval
				this.statusTimer.Interval = 50;
				// TODO: Publish move event
			}
			else
			{
				// The camera has stopped moving

				this.isMoving = false;

				this.statusTimer.Stop();

				// Set update interval back to default
				this.statusTimer.Interval = 100;

				if (this.isMovingToTarget)
				{
					if (this.moveTarget == this.position)
					{
						this.isMovingToTarget = false;
						// Consider publishing event: moving-to finished
					}
					else
					{
						// Consider what scenarios this point is reached
						logger.LogWarning("The camera has stopped moving before reaching its target");
						// TODO: Implement repeat limit and error reporting
						await this.MoveTo(this.moveTarget);
					}
				}
			}
		}

		public async Task<bool> AbsoluteMove(PtzValue position)
		{
			return await Call<bool>("absoluteMove", position);
		}

		public async Task<UriBuilder> GetSnapshotUri()
		{
			string uriString = await Call<string>("getSnapshot");
			// Note that username password is not set
			UriBuilder snapshotUri = new UriBuilder(uriString);
			snapshotUri.Host = config.Uri;
			snapshotUri.Port = config.WebPort;
			return snapshotUri;
		}

		public async Task<string> GetSnapshot()
		{
			// Formatting DateTime: https://stackoverflow.com/questions/7898392/append-timestamp-to-a-file-name

			string filename = $"snapshot_{this.Name}-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.jpg";
			UriBuilder uri = await GetSnapshotUri();

			using (var client = new WebClient())
			{
				// Set credentials explicitly, as the WebClient does not use the credentials from the UriBuilder
				client.Credentials = new NetworkCredential(config.Username, config.Password);

				try
				{
					await client.DownloadFileTaskAsync(uri.Uri, filename);
				}
				catch (Exception e)
				{
					// Make sure username and password is not included in error message
					logger.LogError(e, "Could not download snapshot from {uri.Uri}");
					return null;
				}
			}

			return filename;
		}

		private async Task<T> Call<T>(string function, params object[] args)
		{
			try
			{
				return await nodeService.InvokeExportAsync<T>(nodeFilename, function, args);
			}
			catch (Exception e)
			{
				logger?.LogError(e, $"Error when calling Node function '{function}()': " + e.InnerException.Message ?? e.Message);
				return default(T);
			}
		}

		public async Task MoveTo(CameraPosition position)
		{
			logger.LogInformation("MoveTo: " + position.ToString());
			this.moveTarget = position;
			// TODO: Test for callback error when offline
			// Camera move operations order: x, zoom, y
			await Call<object>("absoluteMove", this.moveTarget.Native);
			this.isMovingToTarget = true;
			this.statusTimer.Start();
		}

		~Camera()
		{
			this.Disable();
			logger?.LogInformation("Deleted");
		}

		// Events

		protected void OnMoving()
		{
			Moving?.Invoke(this, EventArgs.Empty);
		}

		protected void OnStatusChanged()
		{
			// Enabled
			StatusChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
