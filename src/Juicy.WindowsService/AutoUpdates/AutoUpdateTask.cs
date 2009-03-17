
using System;
using System.Collections.Generic;
using System.Text;



namespace Juicy.WindowsService.AutoUpdates
{
	public sealed class AutoUpdateTask : PeriodicalTask
	{
		readonly static log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public AutoUpdateTask(string serviceName, string updaterExePath)
			: this("AutoUpdate", serviceName, updaterExePath)
		{
		}

		public AutoUpdateTask(string taskName, string serviceName, string updaterExePath)
			: base(taskName)
		{
			this.serviceName = serviceName;
			updUtil = new UpdateUtil(updaterExePath);
		}

		private string serviceName;
		private UpdateUtil updUtil;

		public override void Execute()
		{
			List<string> updates = updUtil.GetUpdatedProgramFiles();

			if(updates.Count > 0)
			{
				string msg = "The files below have newer versions:" + Environment.NewLine;
				
				foreach(string f in updates)
					msg += "      " + f + Environment.NewLine;

				Log.Info(msg);

				//now fire the updater and wait to be be killed
				updUtil.StartUpdater(this.serviceName);
			}
		}
	}
}
