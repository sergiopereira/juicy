using System.Configuration.Install;
using System.Reflection;

namespace Juicy.WindowsService
{
	/*
	*  This class was gotten from W. Kevin Hazzard and his article
	*  at http://69.10.233.10/KB/dotnet/WinSvcSelfInstaller.aspx
	*  The ProcessIntallationRequest method is new.
	* */

	public static class SelfInstaller
	{
		private static readonly string _exePath = Assembly.GetEntryAssembly().Location;

		public static bool ProcessIntallationRequest(string[] programArguments)
		{
			bool processed = false;

			if (
			programArguments != null && programArguments.Length == 1
			&& programArguments[0].Length > 1
			&& (programArguments[0][0] == '-' || programArguments[0][0] == '/')
			)
			{
				switch (programArguments[0].Substring(1).ToLower())
				{
					case "install":
					case "i":
						InstallMe();
						processed = true;
						break;

					case "uninstall":
					case "u":
						UninstallMe();
						processed = true;
						break;
				}
			}

			return processed;

		}

		static bool InstallMe()
		{
			try
			{
				ManagedInstallerClass.InstallHelper(new string[] { _exePath });
			}
			catch
			{
				return false;
			}
			return true;
		}

		static bool UninstallMe()
		{
			try
			{
				ManagedInstallerClass.InstallHelper(new string[] { "/u", _exePath });
			}
			catch
			{
				return false;
			}
			return true;
		}
	}

}
