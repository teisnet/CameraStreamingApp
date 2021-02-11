using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnvifCamera
{
	public enum Status { Online, Offline, Disabled }

	readonly public struct CameraStatus
	{
		public CameraStatus(bool enabled, bool online)
		{
			this.Enabled = enabled;
			this.Online = online;
			this.Status = online ? Status.Offline : enabled ? Status.Offline : Status.Disabled;
		}

		public bool Enabled { get; }
		public bool Online { get; }
		public Status Status { get; }
	}
}
