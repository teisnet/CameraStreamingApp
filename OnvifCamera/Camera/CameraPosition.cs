using System;

namespace OnvifCamera
{
	public struct PtzValue
	{
		public PtzValue(float X, float Y, float Zoom)
		{
			this.X = X;
			this.Y = Y;
			this.Zoom = Zoom;
		}

		public float X;
		public float Y;
		public float Zoom;

		public override string ToString() => $"(X={X:0.00}, Y={Y:0.00}, Zoom={Zoom:0.0}]";
	}


	public struct PtzRange
	{
		public Range<float> X;
		public Range<float> Y;
		public Range<float> Zoom;
	}


	// TODO: Investigate operator overload warnings

	public struct CameraPosition : IEquatable<CameraPosition>
	{
		// Private fields

		private PtzValue nativePosition;

		private float accuracy;
		private readonly PtzRange range;

		// public PtzValue Degrees    { get; set; }
		public PtzValue Normalized { get; set; }
		public PtzValue Native { get => nativePosition; set => nativePosition = value; }


		public CameraPosition(PtzRange range) : this()
		{
			this.range = range;
		}

		public bool Equals(CameraPosition other)
		{
			// TODO: Implement accuracy parameter
			return other != null &&
					this.nativePosition.X == other.nativePosition.X &&
					this.nativePosition.Y == other.nativePosition.Y &&
					this.nativePosition.Zoom == other.nativePosition.Zoom;
		}

		public override int GetHashCode() => HashCode.Combine(this.nativePosition);

		public static bool operator ==(CameraPosition pos1, CameraPosition pos2) => Compare(pos1, pos2);
		public static bool operator !=(CameraPosition pos1, CameraPosition pos2) => !Compare(pos1, pos2);

		public static bool Compare(CameraPosition pos1, CameraPosition pos2)
		{
			// TODO: Implement accuracy parameter
			return pos1.Native.X == pos2.Native.X;
		}
	}
}
