using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OnvifCamera
{
	public class Camera : ICamera
	{
		private readonly ILogger<Camera> logger;
		CameraConfig config;
		public string Uri => config.Uri;

		public Camera() { }

		// The dependency injection container will automatically use this constructor.
		public Camera(IOptionsMonitor<CameraConfig> options, ILogger<Camera> logger)
		{
			this.logger = logger;
			this.config = options.CurrentValue;

			options.OnChange(config => {
				this.config = config;
				logger.LogInformation("The camera configuration has been updated.");
			});
		}

		public void Move()
		{

		}
	}
}
