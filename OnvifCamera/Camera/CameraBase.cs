using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OnvifCamera
{
	/*
	 * https://github.com/aspnet/JavaScriptServices/tree/master/src/Microsoft.AspNetCore.NodeServices
	 */

	public class CameraBase
	{
		public string Name => config.Name;

		// iSInitialized should never be set back to false after setting it to true.
		protected bool isInitialized = false;

		public bool IsEnabled { get; protected set; }
		public bool IsOnline { get; protected set; }
		public CameraStatus Status { get => new(IsEnabled, IsOnline); }
		public PtzValue Position { get => position; }

		protected bool pendingStatus;
		protected bool pendingConnect;
		protected bool isMoving;
		protected bool isMovingToTarget;

		// Position , moveTarget and previousPosition in degrees (not internal camera values)
		// TODO: In newing necesary?
		protected PtzValue position = new ();
		protected PtzValue previousPosition = new ();
		protected PtzValue moveTarget = new ();

		private INodeServices nodeService;
		private string nodeFilename = "./node-camera.js";

		protected readonly ILogger<Camera> logger;
		protected CameraConfig config;
		private int configHash;

		public CameraBase()
		{
			// TODO's
			// Create logger manually
		}

		// The dependency injection container will automatically use this constructor.
		public CameraBase(IOptionsMonitor<CameraConfig> config, ILogger<Camera> logger, INodeServices nodeServices)
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
					logger.LogInformation($"Camera[{Name}]: The camera configuration has been updated.");

					OnConfigChange();
				}
			});

			this.nodeService = nodeServices;
		}

		protected void OnConfigChange()
		{
		}

		protected async Task<T> Call<T>(string function, params object[] args)
		{
			try
			{
				return await nodeService.InvokeExportAsync<T>(nodeFilename, function, args);
			}
			catch (Exception e)
			{
				logger.LogError(e, $"Camera[{Name}]: Error when calling Node function '{function}()': " + e.InnerException?.Message ?? e.Message);
				return default;
			}
		}

		private async Task<Uri> GetSnapshotUri()
		{
			string uriString = await Call<string>("getSnapshot");

			UriBuilder snapshotUri = new UriBuilder(uriString);

			// Replace host and port values as they might be LAN specific values.
			snapshotUri.Host = config.Uri;
			snapshotUri.Port = config.WebPort;
			snapshotUri.UserName = config.Username;
			snapshotUri.Password = config.Password;

			return snapshotUri.Uri;
		}

		/// <summary>
		/// Downloads a snapshot from the camera.
		/// </summary>
		/// <returns>Filename of the downloaded snapshot image.</returns>
		public async Task<string> GetSnapshot()
		{
			if (!isInitialized)
			{
				throw new InvalidOperationException($"Cannot get snapshot when camera {Name} is not yet initialized.");
			}

			// Formatting DateTime: https://stackoverflow.com/questions/7898392/append-timestamp-to-a-file-name

			string filename = $"snapshot_{config.Slug}_{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.jpg";
			Uri uri = await GetSnapshotUri();

			using (var client = new WebClient())
			{
				try
				{
					await client.DownloadFileTaskAsync(uri, filename);
				}
				catch (Exception e)
				{
					logger.LogError($"Camera[{Name}]: Could not download snapshot from {uri.Scheme}://{uri.Host}:{uri.Port}{uri.PathAndQuery} (credentials are omitted here)");
					return default;
				}
			}

			return filename;
		}
	}
}
