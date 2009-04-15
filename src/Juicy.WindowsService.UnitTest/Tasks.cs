using System;

namespace Juicy.WindowsService.UnitTest
{
	class MockSchedTask : ScheduledTask
	{
		public MockSchedTask() { }

		protected override void Execute(DateTime scheduledDate)
		{
			Executed = true;
		}

		public bool Executed = false;
	}

	class MockPeriodTask : PeriodicalTask
	{
		public MockPeriodTask() { }


		public override void Execute()
		{
			Executed = true;
		}

		public bool Executed = false;
	}

	class NamedMockSchedTask : ScheduledTask
	{
		public NamedMockSchedTask(string name) : base(name) { }

		protected override void Execute(DateTime scheduledDate)
		{
			Executed = true;
		}

		public bool Executed = false;
	}

	class NamedMockPeriodTask : PeriodicalTask
	{
		public NamedMockPeriodTask(string name) : base(name) { }


		public override void Execute()
		{
			Executed = true;
		}

		public bool Executed = false;
	}
}
