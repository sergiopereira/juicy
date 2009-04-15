using System;
using System.Collections.Generic;
using System.IO;

namespace Juicy.WindowsService.AutoUpdates
{
	/// <summary>
	/// Provides functionality to assist with updating the
	/// application files.
	/// </summary>
	public class UpdateUtil
	{
		readonly static log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="updaterExePath">Complete path to the SvcUpdater.exe file</param>
		public UpdateUtil(string updaterExePath)
		{
			this.updaterExeFileName = Path.GetFileName(updaterExePath).ToLower();
			this.updatesDir = Path.GetDirectoryName(updaterExePath);
		}

		private string updaterExeFileName;
		private string updatesDir;

		const string backupDirFormatStr = "{0}\\backup-{1:yyyy-MM-dd-HH-mm}";

		/// <summary>
		/// Produces the list of the files that have been updated in comparison
		/// with the current running version of the application
		/// </summary>
		/// <returns></returns>
		public List<string> GetUpdatedProgramFiles()
		{
			List<string> list = new List<string>();
			string[] updates = Directory.GetFiles(updatesDir);
			string updaterFileNoExt = Path.GetFileNameWithoutExtension(updaterExeFileName).ToLower();

			foreach(string f in updates)
			{
				string file = Path.GetFileName(f).ToLower();
				//ignore the updater program support files (start with the same file name)
				if(file.StartsWith(updaterFileNoExt)) continue;

				if(CheckForUpdates(file, false, new List<string>()))
					list.Add(file);
			}

			return list;

		}

		/// <summary>
		/// Checks if a file changed and possibly updates it
		/// </summary>
		/// <param name="fileName">Name of the file</param>
		/// <param name="performUpdate">Indicates if the file replacement will happen in case the file has been updated</param>
		/// <param name="filesTobeKept">List files that should not be deleted after the update (the ones that are user by the SvcUpdater.exe process.</param>
		/// <returns>Returns <c>true</c> if a newer version was found</returns>
		public bool CheckForUpdates(string fileName, bool performUpdate, List<string> filesTobeKept)
		{
			if(HasNewerVersion(fileName))
			{
				Log.Debug("Found an updated version of " + fileName);
				if(performUpdate)
					UpdateFile(fileName, filesTobeKept.Contains(fileName));

				return true;
			}

			return false;
		}

		private bool HasNewerVersion(string fileName)
		{
			string current = fileName;
			string candidate = Path.Combine(updatesDir, fileName);

			if(!File.Exists(candidate)) return false;//no newer version available
			if(!File.Exists(current)) return true;//no current version exists

			FileInfo fcurr = new FileInfo(current);
			FileInfo fcand = new FileInfo(candidate);
			//compare dates
			return (fcand.LastWriteTime > fcurr.LastWriteTime);
		}

		/// <summary>
		/// Updates one file
		/// </summary>
		/// <param name="fileName">File to be updated</param>
		/// <param name="keepFile">If <c>true</c> then the file won't be deleted in the end</param>
		public void UpdateFile(string fileName, bool keepFile)
		{
			
			Log.Info("Updating file " + fileName);
			string current = fileName;
			string candidate = Path.Combine(updatesDir, fileName);

			if(File.Exists(current))
			{
				//copy to backup
				string bkpDir = string.Format(backupDirFormatStr, updatesDir, DateTime.Now);

				if(!Directory.Exists(bkpDir))
					Directory.CreateDirectory(bkpDir);

				string bkpFile = Path.Combine(bkpDir, fileName);
				File.Copy(current, bkpFile, true);
			}

			//now bring down the new one
			try
			{
				File.Copy(candidate, current, true);
			}
			catch 
			{
				Log.Warn("Could not update file: " + current);
				throw;
			}
			finally
			{
				//we want to delete the new file even if the
				// update doesn't work
				// BUT avoid trying to delete files that are locked by the updater executable
				if(!keepFile)
				{
					File.Delete(candidate);
					Log.Debug("Deleted file: " + candidate);
				}
			}
		}

		/// <summary>
		/// Launches the SvcUpdater.exe program that will perform the updates
		/// </summary>
		/// <param name="serviceNameToStop">The name of the service being updated</param>
		public void StartUpdater(string serviceNameToStop)
		{
			if(serviceNameToStop == null) serviceNameToStop = "";
			string updater = Path.Combine(Directory.GetCurrentDirectory(), updatesDir);
			updater = Path.Combine(updater, updaterExeFileName);
			Log.Info("Launching updater program: " + updater);

			System.Diagnostics.Process.Start(updater, serviceNameToStop);
		}
	}
}
