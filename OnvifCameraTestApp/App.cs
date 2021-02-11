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
			Console.WriteLine("S: Snapshot, 1: (x=0,y=0,zoom=1), 2:(x=0,y=1,zoom=1), 3:(x=1,y=1,zoom=1)");

			while (true)
			{
				var key = Console.ReadKey(true);
				switch (Char.ToLower(key.KeyChar))
				{
					case '1':
						Console.WriteLine("Moving to { X = 0, Y = 0, Zoom = 1 }");
						await camera.MoveTo(new PtzValue { X = 0, Y = 0, Zoom = 1 });
						break;
					case '2':
						Console.WriteLine("Moving to { X = 0, Y = 1, Zoom = 1 }");
						await camera.MoveTo(new PtzValue { X = 0, Y = 1, Zoom = 1 });
						break;
					case '3':
						Console.WriteLine("Moving to { X = 1, Y = 1, Zoom = 1 }");
						await camera.MoveTo(new PtzValue { X = 1, Y = 1, Zoom = 1 });
						break;
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

				Console.WriteLine("Done");
			}
		}
	}
}
