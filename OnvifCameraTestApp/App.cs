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
		private readonly CameraConfig cameraConfig;
		private readonly AppSettings appSettings;

		public App(IOptions<AppSettings> appSettings, ILogger<App> logger, IOptions<CameraConfig> cameraConfig)
		{
			this.logger = logger;
			this.cameraConfig = cameraConfig.Value;
			this.appSettings = appSettings.Value;
		}

		public async Task Run(string[] args)
		{
			logger.LogInformation("ONVIF CAMERA TEST APP");

			logger.LogInformation($"App title: {appSettings.Title}");
			logger.LogInformation($"Camera name: {cameraConfig.Name}");
			logger.LogInformation($"Camera uri: {cameraConfig.Uri}");
		}
	}
}
