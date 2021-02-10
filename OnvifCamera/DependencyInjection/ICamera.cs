using System;
using System.Threading.Tasks;

namespace OnvifCamera
{
	public interface ICamera
	{
		string Name { get; }

		event EventHandler StatusChanged;

		void Disable();
		Task Enable();
		Task<string> GetSnapshot();
		Task<bool> AbsoluteMove(PtzValue position);
	}
}