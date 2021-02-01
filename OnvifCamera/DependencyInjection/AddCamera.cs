using OnvifCamera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CameraCollectionExtensions
	{
		public static IServiceCollection AddCamera(this IServiceCollection collection, Action<CameraConfig> setupAction)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

			// Read configuration from callback method
			collection.Configure(setupAction);
			return collection.AddSingleton<ICamera, Camera>();
		}
	}
}
