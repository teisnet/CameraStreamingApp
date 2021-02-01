using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OnvifCamera
{
	public class Camera : ICamera
	{
		private readonly ILogger<Camera> logger;
		CameraConfig config;
		private int configHash;

		public string Uri => config.Uri;
		public string Name => config.Uri;

		public Camera() { }

		// The dependency injection container will automatically use this constructor.
		public Camera(IOptionsMonitor<CameraConfig> options, ILogger<Camera> logger)
		{
			this.logger = logger;
			this.config = options.CurrentValue;
			configHash = config.GetHashCode();

			options.OnChange(config => {
				this.config = config;
				logger.LogInformation("The camera configuration has been updated.");

				// For some reason OnChange is fired twice per update. Don't act if the config parameters is the same.
				// See https://github.com/dotnet/aspnetcore/issues/2542
				var newConfigHash = config.GetHashCode();

				if (newConfigHash != configHash)
				{
					this.config = config;
					configHash = newConfigHash;
					logger.LogInformation("The camera configuration has been updated.");
				}
				else
				{
					logger.LogInformation("The camera configuration has not been changed.");
				}
			});
		}

		public void Move()
		{

		}
	}
}
