
using System.ComponentModel;

namespace SampleService
{
	[RunInstaller(true)]
	public class Installer : Juicy.WindowsService.ServiceInstaller
	{
		//That's all we need. Hooray!
	}
}
