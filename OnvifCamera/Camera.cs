using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OnvifCamera
{
	public class Camera : ICamera
	{
		private readonly ILogger<Camera> logger;
		private readonly IOptionsMonitor<CameraConfig> config;
		public string Uri => config.CurrentValue.Uri;

		public Camera() { }

		// The dependency injection container will automatically use this constructor.
		public Camera(IOptionsMonitor<CameraConfig> options, ILogger<Camera> logger)
		{
			this.logger = logger;
			this.config = options;
		}

		public void Move()
		{

		}
	}
}
