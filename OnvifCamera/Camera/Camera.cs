using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO;

namespace OnvifCamera
{
	public enum MoveCommand { Stop, Left, Right, Up, Down, ZoomOut, ZoomIn }

	public class Camera : CameraBase, ICamera
	{
		// Timers
		// Reconnect time
		private Timer heartbeatTimer = new Timer(7000);
		private Timer statusTimer = new Timer(100);

		// Camera state fields

		private JToken nodeOnvifCamera;
		private dynamic nodeOnvifCameraData;


		public dynamic Capabilities { get; set; }
		public dynamic VideoSources { get; set; }
		public dynamic Profiles { get; set; }
		public dynamic DefaultProfile { get; set; }
		public dynamic ActiveSource { get; set; }

		// public PtzRange Range;

		// Events
		public event EventHandler Moving;
		public event EventHandler StatusChanged;

		public Camera(IOptionsMonitor<CameraConfig> config, ILogger<Camera> logger, INodeServices nodeServices)
			: base(config, logger, nodeServices)
		{
			heartbeatTimer.Elapsed += (sender, e) => Connect();
			statusTimer.Elapsed += (sender, e) => UpdateStatus();

			// TODO: Cannot automatically enable on initialization since it cannot be awaited inside a constructor.
			// if (this.config.Enabled)
			// { await Enable(); }
			// else { logger.LogWarning($"{Name}]: disabled"); }
		}

		// Instantiates the Node.js Cam object. Called by Connect() if not already connected.
		private async Task<bool> Init()
		{
			if (isInitialized) return true;

			// The code below will only be executed once.

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

			GetCameraProperties();

			isInitialized = true;

			return isInitialized;
		}

		private void GetCameraProperties()
		{
			nodeOnvifCameraData = nodeOnvifCamera.ToObject<dynamic>();

			Profiles = nodeOnvifCamera["profiles"];
			Capabilities = nodeOnvifCamera["capabilities"];
			VideoSources = nodeOnvifCamera["videoSources"];
			DefaultProfile = nodeOnvifCamera["defaultProfile"];
			ActiveSource = nodeOnvifCamera["activeSource"];

			dynamic ptzConfiguration = DefaultProfile.PTZConfiguration;

			dynamic zoomLimits = ptzConfiguration.zoomLimits.range.XRange;
			float zoomMax = zoomLimits.max.ToObject<float>();
			float zoomMin = zoomLimits.min.ToObject<float>();

			dynamic panTiltLimits = ptzConfiguration.panTiltLimits.range;
			float xMin = panTiltLimits.XRange.min.ToObject<float>();
			float xMax = panTiltLimits.XRange.max.ToObject<float>();
			float yMin = panTiltLimits.YRange.min.ToObject<float>();
			float yMax = panTiltLimits.YRange.max.ToObject<float>();

			/*
			Range = new PtzRange()
			{
				X = new Range<float>(xMin, xMax),
				Y = new Range<float>(yMin, yMax),
				Zoom = new Range<float>(zoomMin, zoomMax)
			};

			moveTarget = new CameraPosition(Range);
			position = new CameraPosition(Range);
			previousPosition = new CameraPosition(Range);
			*/
		}

		public async Task SaveInfo()
		{
			if (!isInitialized) return;

			string cameraInfoJson = JsonConvert.SerializeObject(nodeOnvifCamera, Formatting.Indented);

			await File.WriteAllTextAsync($"Camera {Name} info.json", cameraInfoJson);
		}

		public async Task Enable()
		{
			if (IsEnabled) { return; }
			IsEnabled = true;

			logger.LogInformation($"[{Name}]: Enabled");

			// In case camera is not online, emit 'enabled' at least.
			PublishStatusChanged(/*IsEnabled*/);

			await Connect();
			heartbeatTimer.Start();
		}

		public void Disable()
		{
			if (!IsEnabled) { return; }
			IsEnabled = false;
			// TODO: Stop any movements in progress
			heartbeatTimer.Stop();
			if (IsOnline)
			{
				// SetOnline will only fire if IsOnline has changed.
				SetOnline(false);
			}
			else
			{
				PublishStatusChanged(/*IsEnabled*/);
			}

			logger.LogInformation($"[{Name}]: Disabled");
		}

		private async Task Connect()
		{
			if (pendingConnect || pendingStatus) { return; }

			if (!isInitialized)
			{
				try
				{
					pendingConnect = true;
					await Init();
				}
				catch (Exception e)
				{
					logger.LogError(e, $"[{Name}]: Could not connect to camera using ONVIF");
					return;
				}
				finally
				{
					pendingConnect = false;
				}
			}

			SetOnline(true);

			await UpdateStatus();
		}

		private void SetOnline(bool value)
		{
			if (IsOnline == value) { return; }
			IsOnline = value;

			logger.LogInformation($"[{Name}]: {(IsOnline ? "Connected" : "Disconnected")}");

			PublishStatusChanged(/*IsOnline*/);
		}

		public async Task<PtzValue> GetStatus()
		{
			PtzValue statusPosition = await Call<PtzValue>("getStatus");
			return statusPosition;
		}

		// Consider case when camera hasn't startet moving yet. Consider 'settle' period.
		private async Task UpdateStatus()
		{
			if (pendingStatus || !isInitialized) return;

			// TODO: Consider replacement by thread 'lock'

			PtzValue statusPosition;

			pendingStatus = true;
			
			try
			{
				statusPosition = await Call<PtzValue>("getStatus");
			}
			catch (Exception e)
			{
				logger.LogError(e, $"[{Name}]: Could not update status");
				// TODO: Check exception message
				SetOnline(false);
				return;
			}

			pendingStatus = false;

			logger.LogInformation($"[{Name}]: Status recieved: {statusPosition}");

			previousPosition = position;

			position = CamerUtils.CameraToDegrees(statusPosition);

			// Check if the camera is still moving
			if (!CamerUtils.PosIsEqual(previousPosition, position))
			{
				// The camera is still moving

				isMoving = true;

				// Increase update interval
				statusTimer.Interval = 50;
				
				logger.LogInformation($"[{Name}] move event: {position}");
				
				// TODO: Publish move event
			}
			else
			{
				// The camera has stopped moving

				isMoving = false;

				statusTimer.Stop(); // Not in original code

				// Set update interval back to default
				statusTimer.Interval = 100;

				if (isMovingToTarget)
				{
					if (CamerUtils.PosIsEqual(moveTarget, position))
					{
						isMovingToTarget = false;
						
						logger.LogInformation($"[{Name}] moving to target finished: {position}");

						// Consider publishing event: moving-to finished
					}
					else
					{
						// Consider what scenarios this point is reached
						logger.LogWarning($"[{Name}]: The camera has stopped moving before reaching its target.");
						// TODO: Implement repeat limit and error reporting

						// I don't think this should this be awaited.
						await MoveTo(moveTarget);
					}
				}
			}
		}

		public async Task MoveTo(PtzValue position)
		{
			logger.LogInformation($"[{Name}]: MoveTo: " + position.ToString());
			moveTarget = position;
			// TODO: Test for callback error when offline
			// Camera move operations order: x, zoom, y
			isMovingToTarget = true;
			await Call<object>("absoluteMove", CamerUtils.DegreesToCamera(moveTarget));
			statusTimer.Start();
		}

		public void Move(MoveCommand command)
		{
			logger.LogInformation($"[{Name}].move: {command}");

			PtzValue direction = new PtzValue (0, 0, 0);

			switch (command)
			{
				case MoveCommand.Stop:
					break;
				case MoveCommand.Left:
					direction.X = -1.0f;
					break;
				case MoveCommand.Right:
					direction.X = 1.0f;
					break;
				case MoveCommand.Up:
					direction.Y = 1.0f;
					break;
				case MoveCommand.Down:
					direction.Y = -1.0f;
					break;
				case MoveCommand.ZoomOut:
					direction.Zoom = -1.0f;
					break;
				case MoveCommand.ZoomIn:
					direction.Zoom = 1.0f;
					break;
			}
			// TODO: Test for callback error when offline

			_ = Call<object>("continuousMove", direction);

			isMovingToTarget = false;

			statusTimer.Interval = 100;

			//this._updateStatus();

		}

		~Camera()
		{
			Disable();
			logger.LogInformation($"[{Name}]: Deleted");
		}

		// Events

		protected void OnMoving()
		{
			Moving?.Invoke(this, EventArgs.Empty);
		}

		protected void PublishStatusChanged()
		{
			// Enabled
			StatusChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
