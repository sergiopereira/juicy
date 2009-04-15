using System;
using System.Collections.Generic;
using System.Linq;

namespace Juicy.DirtCheapDaemons.Http
{
	public class MountPointResolver
	{
		public MountPoint Resolve(IEnumerable<MountPoint> mountPoints, string requestedVirtualPath)
		{
			var mounts = from point in mountPoints
						 orderby point.VirtualPath descending
						 select point;

			var mountedPoint =
				mounts.FirstOrDefault(p => MountContainsPath(p, requestedVirtualPath));

			//TODO: if path not mounted, return error code
			return mountedPoint;
		}

		public static bool MountContainsPath(MountPoint mount, string virtualPath)
		{
			if (virtualPath.StartsWith(mount.VirtualPath, StringComparison.OrdinalIgnoreCase))
			{
				if (mount.VirtualPath.Length == virtualPath.Length)
				{
					return true;
				}
				var remainingPath = virtualPath.Substring(mount.VirtualPath.Length);
				return remainingPath.Length == 0 ||
					remainingPath[0] == '/' ||
					remainingPath[0] == '?' ||
					remainingPath[0] == '#';
			}

			return false;
		}
	}
}
