using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Juicy.WindowsService
{
	/// <summary>
	/// Task that runs at a specific time every day
	/// </summary>
	public abstract class ScheduledTask : BaseTask
	{
		//this class is a monitor that only executes the task at
		// the schedule time, once per day.

		readonly static log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public ScheduledTask() : this(null) { }

		public ScheduledTask(string name)
			: base(name)
		{
			string time = "00:00";
			if(this.Settings.TaskProperties.ContainsKey("time") )
				time = this.Settings.TaskProperties["time"];

			this.scheduledTime = TimeSpan.Parse(time);

			this.lastProcessedDate = DateTime.Now.Date.AddDays(-1).Add(this.scheduledTime);
		}

		private Timer _timer;

		/// <summary>
		/// The time of the day when the task will run
		/// </summary>
		public TimeSpan ScheduledTime { get { return scheduledTime; } }	
		private TimeSpan scheduledTime = TimeSpan.MinValue;

		private readonly static Random rnd = new Random();

		/// <summary>
		/// Starts the task, waiting for the time to run
		/// </summary>
		public override void Start()
		{
			base.Start();
			//check every 2 minutes (approximately, to avoid coincidences with other tasks)
			int interval = 1000 * rnd.Next(115, 125);
			if(this.scheduledTime != TimeSpan.MinValue)
				_timer = new Timer(new TimerCallback(timer_Elapsed), null, 100, interval);
		}

		/// <summary>
		/// Stops waiting for the time to run
		/// </summary>
		public override void Stop()
		{
			base.Stop();
			//kill the timer
			if(_timer != null)
			{
				_timer.Dispose();
				_timer= null;
			}
		}

		/// <summary>
		/// Gets the time of the last run
		/// </summary>
		public DateTime LastProcessedDate
		{
			get
			{
				return lastProcessedDate;
			}
		}
		private DateTime lastProcessedDate;

		protected void UpdateProcessedDate()
		{
			lastProcessedDate = DateTime.Now.Date.Add(this.scheduledTime);
		}

		private void timer_Elapsed(object state)
		{
			if(!this.Started) return;

			TimeSpan diff = DateTime.Now - LastProcessedDate;

			if(diff.TotalHours >= 24)
			{
				//looks like it's time to run again
				Log.DebugFormat("Time to execute task {0}" + this.Name);

				if(!this.Enabled)
				{
					Log.InfoFormat("Task {0} is disabled and will not execute." + this.Name);
					return;
				}

				lock(this)
				{
					//double check
					diff = DateTime.Now - LastProcessedDate;
					if(diff.TotalHours >= 24)
					{
						//flag it as executed at the scheduled time today
						this.UpdateProcessedDate();
					}
				}

				//NOTE: we will run this outside the lock() because
				// we never know how long Execute() takes to complete
				if(diff.TotalHours >= 24)
				{
					try
					{
						this.Execute();
					}
					catch(Exception ex)
					{
						//we will log and supress this exception otherwise the
						// whole windows service process comes crashing down
						Log.Warn("The task " + this.Name + " did not handle an exception.", ex);
					}
				}
				
			}
		}

		protected abstract void Execute(DateTime scheduledDate);

		/// <summary>
		/// Runs the daily task
		/// </summary>
		public override void Execute()
		{
			Log.DebugFormat("Will execute task {0}.", this.Name);
			this.Execute(this.LastProcessedDate);
			Log.DebugFormat("Completed task {0}.", this.Name);

		}
	}
}
