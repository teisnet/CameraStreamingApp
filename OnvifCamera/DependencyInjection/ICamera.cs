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

		public bool IsEnabled { get; }
		public bool IsOnline { get; }
		public CameraStatus Status { get; }
		public PtzValue Position { get; }

		public Task MoveTo(PtzValue position);
	}
}