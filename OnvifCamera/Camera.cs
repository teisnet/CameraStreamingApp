using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OnvifCamera
{
	public class Camera : ICamera
	{
		private readonly ILogger<Camera> logger;
		private readonly CameraConfig config;

		public Camera()
		{
			Console.WriteLine("Default constructor");
		}

		// The dependency injection container will automatically use this constructor.
		public Camera(IOptions<CameraConfig> options, ILogger<Camera> logger)
		{
			this.logger = logger;
			this.config = options.Value;

			logger.LogInformation("ILogger constructor");
		}

		public void Move()
		{

		}
	}
}
