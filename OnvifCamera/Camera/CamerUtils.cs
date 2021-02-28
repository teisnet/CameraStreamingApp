using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnvifCamera
{
	public class CamerUtils
	{
		public static PtzValue DegreesToCamera(PtzValue degreesPos)
		{
			return new PtzValue( degreesPos.X * 100f, degreesPos.Y * 100f, degreesPos.Zoom * 1000f );
		}

		// Internal camera values are: x and y = x100, zoom = x1000
		public static PtzValue CameraToDegrees(PtzValue internalPos)
		{
			return new PtzValue( internalPos.X / 100f, internalPos.Y / 100f, internalPos.Zoom / 1000f );
		}

		private static bool IsEqual(float a, float b, float margin = .10f) =>  a > (b - margin) && a < (b + margin);

		public static bool PosIsEqual(PtzValue a, PtzValue b)
		{
			return IsEqual(a.X, b.X) && IsEqual(a.Y, b.Y) && IsEqual(a.Zoom, b.Zoom, .010f);
		}
	}
}
