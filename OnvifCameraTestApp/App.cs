using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnvifCamera;
using System;
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

			TestCamera();
		}

		private void TestCamera()
		{
			camera.Enable();
		}
	}
}
