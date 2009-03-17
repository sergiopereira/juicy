using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.WindowsService
{

	/// <summary>
	/// Decribes a windows service with information to be used at install time
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
	public sealed class ServiceRegistrationAttribute : Attribute
	{
		public ServiceRegistrationAttribute(string serviceName) : this(serviceName, serviceName, "") { }
		public ServiceRegistrationAttribute(string serviceName, string serviceDisplayName, string description)
		{
			this.serviceName = serviceName;
			this.serviceDisplayName = serviceDisplayName;
			this.description = description;
		}

		/// <summary>
		/// Gets or sets the description of the service
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
		private string description;

		/// <summary>
		/// Gets the short name of the service
		/// </summary>
		public string ServiceName
		{
			get { return serviceName; }
		}
		private string serviceName;

		/// <summary>
		/// Gets or sets the name of the service that shows in the Services applet
		/// </summary>
		public string ServiceDisplayName
		{
			get { return serviceDisplayName; }
			set { serviceDisplayName = value; }
		}
		private string serviceDisplayName;
	
	
	}
}
