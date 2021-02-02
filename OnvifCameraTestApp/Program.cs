using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace OnvifCameraTestApp
{
	class Program
	{
		// https://keestalkstech.com/2018/04/dependency-injection-with-ioptions-in-console-apps-in-net-core-2/

		static async Task Main(string[] args)
		{
			var services = new ServiceCollection();
			ConfigureServices(services);
			var serviceProvider = services.BuildServiceProvider();
			await serviceProvider.GetService<App>().Run(args);
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			// Configure logging
			services.AddLogging(configure => {
				configure.AddConsole();
				configure.AddDebug();
			});

			// Build configuration
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false)
				.AddEnvironmentVariables()
				.Build();

			services.Configure<CameraSettings>(configuration.GetSection("Camera"));

			// Add services
			// services.AddTransient<IMyRepository, MyConcreteRepository>();

			// Add app
			services.AddTransient<App>();
		}
	}
}
