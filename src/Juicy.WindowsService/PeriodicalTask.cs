using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Juicy.WindowsService
{
	/// <summary>
	/// A background task that runs periodically
	/// </summary>
	public abstract class PeriodicalTask: BaseTask
	{

		readonly static log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


		public PeriodicalTask() : this(null) { }

		protected PeriodicalTask(string name)
			: base(name)
		{
			string seconds = "0";
			if(this.Settings.TaskProperties.ContainsKey("interval"))
				seconds = this.Settings.TaskProperties["interval"];

			this.interval = int.Parse(seconds);
		}

		private Timer _timer;

		private List<string> executingTasks = new List<string>();

		/// <summary>
		/// Interval in seconds between each execution
		/// </summary>
		public int IntervalSeconds{get{return interval;}}
		private int interval;

		/// <summary>
		/// Starts running the task periodically
		/// </summary>
		public override void Start()
		{
			base.Start();
			StartTimer();
		}

		/// <summary>
		/// Stops the task
		/// </summary>
		public override void Stop()
		{
			base.Stop();
			StopTimer();
		}

		#region Timer support code

		private void StartTimer()
		{
			if(interval > 0)
				_timer = new Timer(new TimerCallback(timer_Elapsed), null, 100, 1000*interval);
		}

		private void StopTimer()
		{
			//kill the timer
			if(_timer != null)
			{
				_timer.Dispose();
				_timer= null;
			}
		}

		private void timer_Elapsed(object state)
		{
			Log.DebugFormat("Time to execute task {0}" + this.Name);

			if(!this.Enabled)
			{
				Log.InfoFormat("Task {0} is disabled and will not execute." + this.Name);
				return;
			}

			bool runTask = false;

			lock(executingTasks)
			{
				bool stillRunning = executingTasks.Contains(this.Name);

				//we don't want to have the same
				// task executing more than once at
				// the same time.
				if(!stillRunning)
				{
					//not running... we can run it then.
					runTask = true;
					executingTasks.Add(this.Name);
				}

			}

			if(runTask)
			{
				//--- no code here, please. Only inside the 'try' --//
				try
				{
					Log.DebugFormat("Will execute task {0}.", this.Name);
					this.Execute();
					Log.DebugFormat("Completed task {0}.", this.Name);
				}
				catch(Exception ex)
				{
					//we will log and supress this exception otherwise the
					// whole windows service process comes crashing down
					Log.Warn("The task " + this.Name + " did not handle an exception.", ex);
				}
				finally
				{
					//we need remove the finished task from the list
					lock(executingTasks)
					{
						executingTasks.Remove(this.Name);
					}
				}
			}
			else
			{
				Log.InfoFormat("Task {0} still hasn't finished previous run. Will not execute this time.", this.Name);
			}
		}

		#endregion



	}
}
