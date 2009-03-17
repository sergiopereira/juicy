using System;

namespace Juicy.WindowsService
{
	/// <summary>
	/// Defines the task objects that can be managed by the service
	/// </summary>
	public interface ITask: IDisposable
	{
		bool Started { get; }
		string Name { get; }
		void Start();
		void Stop();
		void Execute();
	}
}
