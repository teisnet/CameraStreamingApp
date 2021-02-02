using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace OnvifCameraTestApp
{
	public class App
	{
		private readonly ILogger<App> logger;
		private readonly AppSettings appSettings;

		public App(IOptions<AppSettings> appSettings, ILogger<App> logger)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
			this.appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
		}

		public async Task Run(string[] args)
		{
			logger.LogInformation("Starting...");

			Console.WriteLine("Hello world!");
			Console.WriteLine(appSettings.TempDirectory);
		}
	}
}
