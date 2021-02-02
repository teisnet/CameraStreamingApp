namespace OnvifCamera
{
	public partial class CameraConfig
	{
		// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type#class-example

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CameraConfig);
		}
		public override int GetHashCode()
		{
			// return Uri.GetHashCode(); // * 0x00010000 + Y;

			// https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-overriding-gethashcode

			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				// Suitable nullity checks etc, of course :)
				hash = hash * 23 + Uri.GetHashCode();
				hash = hash * 23 + Name.GetHashCode();
				return hash;
			}
		}

		/*
		public bool Equals(CameraConfig c)
		{
			// If parameter is null, return false.
			if (Object.ReferenceEquals(c, null))
			{
				return false;
			}

			// Optimization for a common success case.
			if (Object.ReferenceEquals(this, c))
			{
				return true;
			}

			// Check properties that this class declares.
			if (Uri == c.Uri)
			{
				// Let base class check its own fields
				// and do the run-time type comparison.
				return base.Equals((CameraConfig)c);
			}
			else
			{
				return false;
			}
		}

		public static bool operator ==(CameraConfig lhs, CameraConfig rhs)
		{
			// Check for null.
			if (Object.ReferenceEquals(lhs, null))
			{
				if (Object.ReferenceEquals(rhs, null))
				{
					// null == null = true.
					return true;
				}

				// Only the left side is null.
				return false;
			}
			// Equals handles the case of null on right side.
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CameraConfig lhs, CameraConfig rhs)
		{
			return !(lhs == rhs);
		}
		*/
	}
}
