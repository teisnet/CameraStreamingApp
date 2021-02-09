namespace OnvifCamera
{
	public partial class CameraConfig
	{
		public string Name { get; set; }
		public string Uri { get; set; }
		// The web port number is used for snapshot image download.
		public int OnvifPort { get; set; }
		public int WebPort { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
