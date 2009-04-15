using System;
using System.Collections.Generic;
using System.Configuration;

namespace Juicy.WindowsService
{
	public class WindowsServiceConfiguration : ConfigurationSection
	{
		[ConfigurationProperty("tasks")]
		public TaskSettingsCollection TaskSettings { get { return this["tasks"] as TaskSettingsCollection; } }

	}

	public class TaskSettings : ConfigurationElement
	{
		public Dictionary<string, string> TaskProperties
		{ get { return taskProperties; } }
		private Dictionary<string, string> taskProperties = new Dictionary<string, string>();

		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get
			{
				return this["name"] as string;
			}
		}

		[ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
		public bool Enabled { get { return ((bool)this["enabled"]); } }

		public string GetSetting(string settingName)
		{
			return this[settingName] as string;
		}

		protected int GetInt32(string settingName, int defaultValue)
		{
			string s = GetSetting(settingName);
			int res = defaultValue;
			return int.TryParse(s, out res) ? res : defaultValue;
		}

		protected TimeSpan GetTimeSpan(string settingName, TimeSpan defaultValue)
		{
			string s = GetSetting(settingName);
			TimeSpan res = defaultValue;
			return TimeSpan.TryParse(s, out res) ? res : defaultValue;
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			//Accept all unknown attributes..add them to a collection
			taskProperties.Add(name, value);
			return true;
		}

	}


	[ConfigurationCollection(typeof(TaskSettings), AddItemName = "task")]
	public class TaskSettingsCollection : ConfigurationElementCollection
	{
		public TaskSettings this[int index]
		{
			get
			{
				return base.BaseGet(index) as TaskSettings;
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				this.BaseAdd(index, value);
			}
		}

		new public TaskSettings this[string name]
		{
			get
			{
				return this.BaseGet(name) as TaskSettings;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new TaskSettings();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TaskSettings)element).Name;
		}



	}
}
