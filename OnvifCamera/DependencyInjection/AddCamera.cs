using OnvifCamera;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CameraCollectionExtensions
	{
		// https://csharp.christiannagel.com/2016/07/27/diwithoptions/amp/

		[Obsolete]
		public static IServiceCollection AddCamera(this IServiceCollection services, Action<CameraConfig> setupCallback)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (setupCallback == null) throw new ArgumentNullException(nameof(setupCallback));

			// Read configuration from callback method
			services.Configure(setupCallback);

			services.AddNodeServices(options => {
				// options.LaunchWithDebugging = true;
				// options.DebuggingPort = 9229;
				options.WatchFileExtensions = new [] { "" };
			});

			services.AddSingleton<ICamera, Camera>();

			return services;
		}
	}
}
