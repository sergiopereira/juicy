﻿using Juicy.WindowsService;

namespace SampleService.Tasks
{
	class CleanupTask : PeriodicalTask
    {
		readonly static log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public override void Execute()
        {
            //time to run.... 
            //TODO: write the actual code here
			// ShoppingCart.DeleteAbandonedCarts();
			Log.InfoFormat("Executed: {0}", this.GetType().Name);
		}
    }
}
