using System;
using System.Threading.Tasks;

namespace OnvifCamera
{
	public interface ICamera
	{
		dynamic ActiveSource { get; set; }
		dynamic Capabilities { get; set; }
		dynamic DefaultProfile { get; set; }
		string Name { get; }
		dynamic Profiles { get; set; }
		string Uri { get; }
		dynamic VideoSources { get; set; }

		event EventHandler Moving;
		event EventHandler StatusChanged;

		Task<bool> AbsoluteMove(PtzValue position);
		void Disable();
		Task Enable();
		Task<string> GetSnapshot();
		Task MoveTo(CameraPosition position);
	}
}