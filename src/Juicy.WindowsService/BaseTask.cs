using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Juicy.WindowsService
{
	/// <summary>
	/// Represents a generic background task
	/// </summary>
	public abstract class BaseTask : ITask
	{
		/// <summary>
		/// Creates a new instance of a generic task
		/// </summary>
		/// <param name="name"></param>
		protected BaseTask(string name)
		{
			if(string.IsNullOrEmpty(name)) name = this.GetType().Name;

			this.name = name;
			Console.WriteLine(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
			WindowsServiceConfiguration conf = ConfigurationManager.GetSection("WindowsService") as WindowsServiceConfiguration;
			if(conf == null)
				throw new ConfigurationErrorsException("Could not find the mandatory <WindowsService /> configuration section.");

			this.settings = conf.TaskSettings[name];
		}

		/// <summary>
		/// Gets the configuration settings for this task
		/// </summary>
		public virtual TaskSettings Settings { get { return settings; } }
		private TaskSettings settings;
	
		/// <summary>
		/// Indicates if the task is enabled (will run).
		/// </summary>
		public bool Enabled { get { return this.settings.Enabled; } }

		/// <summary>
		/// Indicates if the task has been started
		/// </summary>
		public bool Started { get { return started; } }
		private bool started;

		/// <summary>
		/// Gets or sets the name of the task
		/// </summary>
		public string Name { get { return name; } set { name = value; } }
		private string name;

		/// <summary>
		/// Starts the task
		/// </summary>
		public virtual void Start()
		{
			started = true;
		}

		/// <summary>
		/// Stops the task
		/// </summary>
		public virtual void Stop()
		{
			started = false;
		}

		/// <summary>
		/// Stops and releases any resources kept by the task
		/// </summary>
		public virtual void Dispose()
		{
			this.Stop();
		}

		/// <summary>
		/// Executes the work performed by the task
		/// </summary>
		public abstract void Execute();
	}
}