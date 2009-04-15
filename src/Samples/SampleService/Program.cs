using Juicy.WindowsService;
using SampleService.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SampleService
{
	class Program
	{
		static void Main(string[] args)
		{
			if (!SelfInstaller.ProcessInstallationRequest(args))
			{

				MyAppService svc = new MyAppService();

				svc.AddTask(new CheckQueueTask());
				svc.AddTask(new CleanupTask());
				svc.AddTask(new DailyReportTask());

				svc.Run();
			}
		}
	}
}
