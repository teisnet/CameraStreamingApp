using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace OnvifCameraTestApp
{
	public class App
	{
		private readonly ILogger<App> logger;
		private readonly CameraSettings cameraSettings;

		public App(IOptions<CameraSettings> appSettings, ILogger<App> logger)
		{
			this.logger = logger;
			this.cameraSettings = appSettings.Value;
		}

		public async Task Run(string[] args)
		{
			logger.LogInformation("ONVIF CAMERA TEST APP");

			logger.LogInformation($"Camera name: {cameraSettings.Name}");
			logger.LogInformation($"Camera uri: {cameraSettings.Uri}");
		}
	}
}
