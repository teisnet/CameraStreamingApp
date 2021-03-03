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

			_ = camera.SaveInfo();

			while (true)
			{
				var key = Console.ReadKey(true);
				switch (key.Key)
				{
					case ConsoleKey.Escape:
						camera.Move(MoveCommand.Stop);
						break;
					case ConsoleKey.LeftArrow:
						camera.Move(MoveCommand.Left);
						break;
					case ConsoleKey.RightArrow:
						camera.Move(MoveCommand.Right);
						break;
					case ConsoleKey.UpArrow:
						camera.Move(MoveCommand.Up);
						break;
					case ConsoleKey.DownArrow:
						camera.Move(MoveCommand.Down);
						break;
					case ConsoleKey.OemPlus:
						camera.Move(MoveCommand.ZoomIn);
						break;
					case ConsoleKey.OemMinus:
						camera.Move(MoveCommand.ZoomOut);
						break;
					case ConsoleKey.D1:
						// Forward
						Console.WriteLine("Moving to { X = 90, Y = 0, Zoom = 1 }");
						await camera.MoveTo(new PtzValue { X = 90, Y = 0, Zoom = 1 });
						break;
					case ConsoleKey.D2:
						// Down
						Console.WriteLine("Moving to { X = 0, Y = 90, Zoom = 1 }");
						await camera.MoveTo(new PtzValue { X = 90, Y = 90, Zoom = 1 });
						break;
					case ConsoleKey.D3:
						// Left
						Console.WriteLine("Moving to { X = 0, Y = 0, Zoom = 1 }");
						await camera.MoveTo(new PtzValue { X = 0, Y = 0, Zoom = 1 });
						break;
					case ConsoleKey.D4:
						// Right
						Console.WriteLine("Moving to { X = 180, Y = 0, Zoom = 1 }");
						await camera.MoveTo(new PtzValue { X = 180, Y = 0, Zoom = 1 });
						break;
					case ConsoleKey.D5:
						// Back 
						Console.WriteLine("Moving to { X = 180, Y = 90, Zoom = 1 }");
						await camera.MoveTo(new PtzValue { X = 270, Y = 0, Zoom = 1 });
						break;
					case ConsoleKey.S:
						string snatshotUri = await camera.GetSnapshot();
						Console.WriteLine($"SnapshotUri = {snatshotUri}");

						// Open snapshot in image viewer
						var filePath = Path.Combine(appSettings.DownloadFolder, snatshotUri);
						Process.Start(appSettings.ImageViewerPath, $"\"{filePath}\"");

						break;
					case ConsoleKey.G:
						PtzValue status = await camera.GetStatus();
						Console.WriteLine($"Status = {status}");
						break;
					case ConsoleKey.X:
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
