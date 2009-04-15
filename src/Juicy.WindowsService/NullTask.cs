
namespace Juicy.WindowsService
{
	/// <summary>
	/// A task that doesn't do anything. It's useful as a placeholder or 
	/// during tests.
	/// </summary>
	public class NullTask : BaseTask
	{
		readonly static log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public NullTask() : this("NullTask") { }
		public NullTask(string name) : base(name) { }

		/// <summary>
		/// Does nothing
		/// </summary>
		public override void Execute()
		{
			Log.Debug("NullTask executed.");
		}
	}
}
