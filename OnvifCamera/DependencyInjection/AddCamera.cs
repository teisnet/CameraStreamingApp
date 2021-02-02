using OnvifCamera;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CameraCollectionExtensions
	{
		// https://csharp.christiannagel.com/2016/07/27/diwithoptions/amp/

		public static IServiceCollection AddCamera(this IServiceCollection collection, Action<CameraConfig> setupAction)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

			// Read configuration from callback method
			collection.Configure(setupAction);

			collection.AddNodeServices(options => { });

			collection.AddSingleton<ICamera, Camera>();

			return collection;
		}
	}
}
