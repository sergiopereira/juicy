using System;
using System.Collections;
using System.Configuration.Install;

namespace Juicy.WindowsService
{

	/// <summary>
	/// Base service installer. Every service needs an installer class that inherits from
	/// this one in order to be possible to use InstallUtil.exe to install the service
	/// </summary>
	public class ServiceInstaller : Installer
	{
		private System.ServiceProcess.ServiceProcessInstaller processInstaller;
		private System.ServiceProcess.ServiceInstaller svcInstaller;
		private string serviceName, serviceDescription, serviceDisplayName;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		public ServiceInstaller()
		{

			GetConfigData();

			// This call is required by the Designer.
			InitializeComponent();
		}

		private void GetConfigData()
		{

			ServiceRegistrationAttribute[] atts =
				(ServiceRegistrationAttribute[])this.GetType().Assembly.GetCustomAttributes(
					typeof(ServiceRegistrationAttribute), false
					);

			if (atts == null || atts.Length == 0)
				throw new ApplicationException("Cannot install the service because the assembly " +
					this.GetType().Assembly.GetName().Name + " does not contain one attribute of type " +
					typeof(ServiceRegistrationAttribute).FullName + ".");

			serviceDescription = atts[0].Description;
			serviceDisplayName = atts[0].ServiceDisplayName;
			serviceName = atts[0].ServiceName;
		}

		public override void Install(IDictionary stateServer)
		{
			Microsoft.Win32.RegistryKey system,
				//HKEY_LOCAL_MACHINE\Services\CurrentControlSet
				currentControlSet,
				//...\Services
				services,
				//...\<Service Name>
				service,
				//...\Parameters - this is where you can put service-specific configuration
				config;

			try
			{
				//Let the project installer do its job
				base.Install(stateServer);

				//Open the HKEY_LOCAL_MACHINE\SYSTEM key
				system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System");
				//Open CurrentControlSet
				currentControlSet = system.OpenSubKey("CurrentControlSet");
				//Go to the services key
				services = currentControlSet.OpenSubKey("Services");
				//Open the key for your service, and allow writing
				service = services.OpenSubKey(this.svcInstaller.ServiceName, true);
				//Add your service's description as a REG_SZ value named "Description"
				// ********** This is where we set the Description!
				service.SetValue("Description", serviceDescription);
				//(Optional) Add some custom information your service will use...
				config = service.CreateSubKey("Parameters");
			}
			catch (Exception e)
			{
				Console.WriteLine("An exception was thrown during service installation:\n" + e.ToString());
			}
		}

		public override void Uninstall(IDictionary stateServer)
		{
			Microsoft.Win32.RegistryKey system,
				currentControlSet,
				services,
				service;

			try
			{
				//Drill down to the service key and open it with write permission
				system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System");
				currentControlSet = system.OpenSubKey("CurrentControlSet");
				services = currentControlSet.OpenSubKey("Services");
				service = services.OpenSubKey(this.svcInstaller.ServiceName, true);
				//Delete any keys you created during installation (or that your service created)
				service.DeleteSubKeyTree("Parameters");
				//...
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception encountered while uninstalling service:\n" + e.ToString());
			}
			finally
			{
				//Let the project installer do its job
				base.Uninstall(stateServer);
			}
		}


		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.processInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.svcInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// processInstaller
			// 
			this.processInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.processInstaller.Password = null;
			this.processInstaller.Username = null;
			// 
			// svcInstaller
			// 
			this.svcInstaller.DisplayName = this.serviceDisplayName;
			this.svcInstaller.ServiceName = this.serviceName;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
																					  this.processInstaller,
																					  this.svcInstaller});

		}
	}
}
