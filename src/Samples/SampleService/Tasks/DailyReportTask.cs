using System;
using Juicy.WindowsService;

namespace SampleService.Tasks
{
    class DailyReportTask : ScheduledTask
    {
		readonly static log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		protected override void Execute(DateTime scheduledDate)
        {
            //time to run.... 
            //TODO: write the actual code here
			// SalesReport.SendDailySummary();
			Log.InfoFormat("Executed: {0}", this.GetType().Name);
		}
    }
}
