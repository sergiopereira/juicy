using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Text;

namespace Juicy.WindowsService
{
	/// <summary>
	/// Base class for Windows Services
	/// </summary>
	public class SPServiceBase: ServiceBase
	{
		readonly static log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private List<ITask> tasks = new List<ITask>();

		/// <summary>
		/// Creates a new instance of the service
		/// </summary>
		public SPServiceBase()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();

			string workingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			Directory.SetCurrentDirectory(workingDirectory);

		}

		/// <summary>
		/// Adds a task to be managed by the service
		/// </summary>
		/// <param name="task"></param>
		public void AddTask(ITask task) { tasks.Add(task); }

		/// <summary>
		/// Removes all the tasks that are being managed by the service
		/// </summary>
		public void ClearTasks() 
		{
			if(this.started)
				throw new InvalidOperationException("Cannot clear the tasks with the service started.");

			tasks.Clear(); 
		}

		private bool started;

		/// <summary>
		/// Starts the service
		/// </summary>
		public void Run()
		{
			try
			{
				Run(this);
			}
			catch(Exception ex)
			{
				Log.Warn("Service " + this.ServiceName + " did not handle exception.", ex);
				throw;
			}
		}

		/// <summary>
		/// Runs the service as a console application. Useful during development
		/// </summary>
		/// <param name="tasksToRun"></param>
		public void RunAsStandAlone(params ITask[] tasksToRun)
		{
			//run each passed task once
			foreach(ITask t in tasksToRun)
			{
				using(t)
				{
					t.Execute();
				}
			}

		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CanPauseAndContinue = true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				foreach(ITask m in tasks)
					m.Dispose();

				if (components != null) 
					components.Dispose();

			}
			base.Dispose( disposing );
		}



		/// <summary>
		/// Set things in motion so the service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			StartBackgroundTasks();
			Log.Info("Service started");
			started = true;
		}

		/// <summary>
		/// Resumes the service
		/// </summary>
		protected override void OnContinue()
		{
			StartBackgroundTasks();
			Log.Info("Service resumed");
		}

		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			StopBackgroundTasks();
			Log.Warn("Service stopped");
			started = false;
		}

		/// <summary>
		/// Pauses the service
		/// </summary>
		protected override void OnPause()
		{

			StopBackgroundTasks();
			Log.Warn("Service paused");
		}

		private void StartBackgroundTasks()
		{
			foreach(ITask m in tasks)
				m.Start();

		}

		private void StopBackgroundTasks()
		{
			foreach(ITask m in tasks)
				m.Stop();
		}


		
	}
}
