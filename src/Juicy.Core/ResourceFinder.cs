using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Juicy
{
	public class ResourceFinder
	{
		public ResourceFinder(Type typeInResourceNamespace)
			: this(typeInResourceNamespace.Assembly, typeInResourceNamespace.Namespace)
		{

		}

		public ResourceFinder(Assembly resourceAssembly, string resourcesNamespace)
		{
			ResourceAssembly = resourceAssembly;
			ResourcesNamespace = resourcesNamespace;
		}

		private Assembly ResourceAssembly { get; set; }
		private string ResourcesNamespace { get; set; }

		public bool Exists(string resourceFileName)
		{
			using (var stream = GetStream(resourceFileName))
			{
				return stream != null;
			}
		}

		public Stream GetStream(string resourceFileName)
		{
			return ResourceAssembly.GetManifestResourceStream(ResourcesNamespace + "." + resourceFileName);
		}

		public string GetText(string resourceFileName)
		{
			using (Stream resStream = GetStream(resourceFileName))
			{
				return new StreamReader(resStream).ReadToEnd();
			}
		}

		public byte[] GetBytes(string resourceFileName)
		{
			List<byte> bytes = new List<byte>();
			using (Stream resStream = GetStream(resourceFileName))
			{
				byte[] buffer = new byte[1000];
				int length = resStream.Read(buffer, 0, buffer.Length);
				while (length > 0)
				{
					byte[] truncated = new byte[length];
					Array.Copy(buffer, truncated, length);
					bytes.AddRange(truncated);
					length = resStream.Read(buffer, 0, buffer.Length);
				}
			}

			return bytes.ToArray();
		}
	}
}
