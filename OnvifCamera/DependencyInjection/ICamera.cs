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

		bool IsEnabled { get; }
		bool IsOnline { get; }
		CameraStatus Status { get; }
		PtzValue Position { get; }

		Task MoveTo(PtzValue position);
		void Move(MoveCommand command);
		Task<PtzValue> GetStatus();
		Task SaveInfo();
	}
}