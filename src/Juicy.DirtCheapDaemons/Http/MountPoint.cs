
namespace Juicy.DirtCheapDaemons.Http
{
	public class MountPoint
	{

		public IMountPointHandler Handler { get; set; }
		public string VirtualPath { get; set; }
	}
}
