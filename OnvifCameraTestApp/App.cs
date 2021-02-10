using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnvifCamera;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace OnvifCameraTestApp
{
	public class App
	{
		private readonly ILogger<App> logger;
		private readonly ICamera camera;
		private readonly CameraConfig cameraConfig;
		private readonly AppSettings appSettings;

		public App(IOptions<AppSettings> appSettings, ILogger<App> logger, IOptions<CameraConfig> cameraConfig, ICamera camera)
		{
			this.logger = logger;
			this.camera = camera;
			this.cameraConfig = cameraConfig.Value;
			this.appSettings = appSettings.Value;

			this.appSettings.DownloadFolder = this.appSettings.DownloadFolder ?? Directory.GetCurrentDirectory();

			camera.StatusChanged += Camera_StatusChanged;
		}

		private void Camera_StatusChanged(object sender, EventArgs e)
		{
			logger.LogInformation("Camera status changed");
		}

		public async Task Run(string[] args)
		{
			logger.LogInformation("ONVIF CAMERA TEST APP");

			logger.LogInformation($"App title: {appSettings.Title}");
			logger.LogInformation($"Camera name: {cameraConfig.Name}");
			logger.LogInformation($"Camera uri: {cameraConfig.Uri}");

			await TestCamera();
		}

		private async Task TestCamera()
		{
			await camera.Enable();

			Console.WriteLine("Press key");

			while (true)
			{
				var key = Console.ReadKey(true);
				switch (Char.ToLower(key.KeyChar))
				{
					case 's':
						string snatshotUri = await camera.GetSnapshot();
						Console.WriteLine($"SnapshotUri = {snatshotUri}");

						// Open snapshot in image viewer
						var filePath = Path.Combine(appSettings.DownloadFolder, snatshotUri);
						Process.Start(appSettings.ImageViewerPath, $"\"{filePath}\"");

						break;
					case 'x':
						return;
					default:
						Console.WriteLine($"Unknown command '{key.KeyChar}'");
						break;

				}
			}
		}
	}
}
