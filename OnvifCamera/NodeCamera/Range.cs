using System;

namespace NodeCameraLib
{
	/*
	 * Range type: https://stackoverflow.com/questions/5343006/is-there-a-c-sharp-type-for-representing-an-integer-range
	 */

	/// <summary>The Range class.</summary>
	/// <typeparam name="T">Generic parameter.</typeparam>
	public class Range<T> where T : IComparable<T>
	{
		/// <summary>Minimum value of the range.</summary>
		public T Min { get; set; }

		/// <summary>Maximum value of the range.</summary>
		public T Max { get; set; }

		public Range() { }

		public Range(T min, T max)
		{
			this.Min = min;
			this.Max = max;
		}


		/// <summary>Presents the Range in readable format.</summary>
		/// <returns>String representation of the Range</returns>
		public override string ToString()
		{
			return string.Format("[{0} - {1}]", this.Min, this.Max);
		}

		/// <summary>Determines if the range is valid.</summary>
		/// <returns>True if range is valid, else false</returns>
		public bool IsValid()
		{
			return this.Min.CompareTo(this.Max) <= 0;
		}

		/// <summary>Determines if the provided value is inside the range.</summary>
		/// <param name="value">The value to test</param>
		/// <returns>True if the value is inside Range, else false</returns>
		public bool ContainsValue(T value)
		{
			return (this.Min.CompareTo(value) <= 0) && (value.CompareTo(this.Max) <= 0);
		}

		public T Confine(T value)
		{
			// TODO:
			return value;
		}
	}
}
