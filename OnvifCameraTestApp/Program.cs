using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using OnvifCamera;
using System.Globalization;

namespace OnvifCameraTestApp
{
	class Program
	{
		// https://keestalkstech.com/2018/04/dependency-injection-with-ioptions-in-console-apps-in-net-core-2/

		static async Task Main(string[] args)
		{
			var services = new ServiceCollection();
			ConfigureServices(services);
			Configure();
			var serviceProvider = services.BuildServiceProvider();
			await serviceProvider.GetService<App>().Run(args);
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			// Configure logging
			services.AddLogging(configure => {
				configure.AddSimpleConsole(c =>
					c.SingleLine = true);
				configure.AddDebug();
			});

			// Build configuration
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false)
				.AddEnvironmentVariables()
				.Build();

			services.Configure<AppSettings>(configuration.GetSection("App"));
			services.Configure<CameraConfig>(configuration.GetSection("Camera"));

			// Add services
			services.AddCamera(config => {
				// config.CurrentCamera = "Fjellebroen";
			});

			// Add app
			services.AddTransient<App>();
		}

		private static void Configure()
		{
			// Don't use commas when outputting float values.
			var cultureInfo = new CultureInfo("en-US");
			CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
			CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
		}
	}
}
