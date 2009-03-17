using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SampleService.Tasks;
using System.Configuration;
using Juicy.WindowsService;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SampleService
{
	class Program
	{
		static void Main(string[] args)
		{
			if (!SelfInstaller.ProcessIntallationRequest(args))
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
