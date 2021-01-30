using System;
using Microsoft.Extensions.Logging;

namespace OnvifCamera
{
	public class Camera : ICamera
	{
		private readonly ILogger<Camera> logger;

		public Camera()
		{
			Console.WriteLine("Default constructor");
		}

		// The dependency injection container will automatically use this constructor.
		public Camera(ILogger<Camera> logger)
		{
			this.logger = logger;

			logger.LogInformation("ILogger constructor");
		}

		public void Move()
		{

		}
	}
}
