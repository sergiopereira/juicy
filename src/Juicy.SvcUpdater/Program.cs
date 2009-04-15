using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Management;
using System.ServiceProcess;
using Juicy.WindowsService.AutoUpdates;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "SvcUpdater.Log4Net.config", Watch = true)]

namespace Juicy.SvcUpdater
{
	class Program
	{
		readonly static log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		static void Main(string[] args)
		{
			try
			{
				if (args.Length == 0)
				{
					Console.WriteLine("Usage: SvcUpdater.exe [serviceName]");
					return;
				}

				string serviceName = args[0];
				string servicePath = GetServiceExePath(serviceName).Trim('"');
				Log.Debug("Service path: " + servicePath);
				string serviceDirectory = Path.GetDirectoryName(servicePath);
				string serviceExe = Path.GetFileName(servicePath);

				string updaterExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
				string updaterExeDir = Path.GetDirectoryName(updaterExePath);

				DirectoryInfo dirInfo = new DirectoryInfo(updaterExeDir);
				string updatesDir = dirInfo.Name;

				//the code assumes the updater runs 
				//with the current directory being the service's exe directory
				Directory.SetCurrentDirectory(serviceDirectory);

				//check if any file changed different than the updater

				UpdateUtil upd = new UpdateUtil(updaterExePath);
				List<string> updates = upd.GetUpdatedProgramFiles();
				//remove files that only support the updater program
				if (updates.Contains("svcupdater.exe.log4net.config")) updates.Remove("log4net.config");
				if (updates.Contains("svcupdater.exe.config")) updates.Remove("svcupdater.config");

				if (updates.Count > 0)
				{
					//stop the service
					Log.Debug("Stopping " + serviceName);
					string status = StopService(serviceName);
					Log.InfoFormat("Result of stopping {0}: {1}", serviceName, status);
					System.Threading.Thread.Sleep(15000);

					//wait until the OS shuts down the process
					int count = 0;
					while (true)
					{
						count++;
						IList list = System.Diagnostics.Process.GetProcessesByName(serviceExe);
						if (list == null || list.Count == 0) break;
						Log.Debug("Waiting for service to shutdown");
						System.Threading.Thread.Sleep(200);
						//we may give up it takes too long (30 secs.)
						if (count > 150) throw new ApplicationException("Service process did not shut down.");
					}

					try
					{
						List<string> keepThese = GetFilesToBeKept();
						//	update and backup each new file
						foreach (string f in updates)
						{
							upd.UpdateFile(f, keepThese.Contains(f.ToLower()));
						}
					}
					finally
					{
						//let's try to leave with the service started
						Log.Debug("Starting " + serviceName);
						string st = StartService(serviceName);
						Log.InfoFormat("Result of starting {0}: {1}", serviceName, st);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Unhandled exception", ex);
				Environment.Exit(1);//error reported 
			}
		}

		private static string GetServiceExePath(string serviceName)
		{
			string wmiObj = "Win32_Service.name='" + serviceName + "'";
			using (ManagementObject serviceInfo = new ManagementObject(wmiObj))
			{
				return serviceInfo.GetPropertyValue("PathName").ToString();
			}
		}

		private static string StopService(string serviceName)
		{
			ServiceController svc = new ServiceController(serviceName);
			if (svc.Status == System.ServiceProcess.ServiceControllerStatus.Running)
			{
				svc.Stop();
				svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
			}
			return svc.Status.ToString();
		}

		private static string StartService(string serviceName)
		{
			ServiceController svc = new ServiceController(serviceName);
			if (svc.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
			{
				svc.Start();
				svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
			}
			return svc.Status.ToString();
		}

		private static List<string> GetFilesToBeKept()
		{
			List<string> list = new List<string>();
			int fileNumber = 1;
			string fileName = ConfigurationManager.AppSettings["fileToKeep" + fileNumber];

			while (!string.IsNullOrEmpty(fileName))
			{
				Log.Debug("Will keep file: " + fileName);
				list.Add(fileName.ToLower());
				fileNumber++;
				fileName = ConfigurationManager.AppSettings["fileToKeep" + fileNumber];
			}

			return list;
		}
	}
}
