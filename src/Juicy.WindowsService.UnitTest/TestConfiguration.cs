using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Juicy.WindowsService.UnitTest
{
	[TestFixture]
	public class TestConfiguration
	{

		[Test]
		public void ShouldConfigurePeriodicalTaskWithDefaults()
		{
			
			MockPeriodTask t = new MockPeriodTask();
			Assert.AreEqual("MockPeriodTask", t.Name);
			Assert.AreEqual(0, t.IntervalSeconds);
			Assert.IsTrue(t.Enabled);
		}

		[Test]
		public void ShouldConfigureScheduledTaskWithDefaults()
		{
			MockSchedTask s = new MockSchedTask();
			Assert.AreEqual("MockSchedTask", s.Name);
			Assert.AreEqual(TimeSpan.Parse("00:00"), s.ScheduledTime);
			Assert.IsTrue(s.Enabled);
		}

		[Test]
		public void ShouldConfigurePeriodicalTaskWithOverrides()
		{

			NamedMockPeriodTask t = new NamedMockPeriodTask("PeriodicalTask1");
			Assert.AreEqual("PeriodicalTask1", t.Name);
			Assert.AreEqual(400, t.IntervalSeconds);
			Assert.IsFalse(t.Enabled);
		}

		[Test]
		public void ShouldConfigureScheduledTaskWithOverrides()
		{
			NamedMockSchedTask s = new NamedMockSchedTask("SchedTask1");
			Assert.AreEqual("SchedTask1", s.Name);
			Assert.AreEqual(TimeSpan.Parse("23:45"), s.ScheduledTime);
			Assert.IsFalse(s.Enabled);
		}

		[Test]
		public void ShouldConfigureScheduledTaskWithExtraAttributes()
		{
			NamedMockSchedTask s = new NamedMockSchedTask("SchedTask1");
			Assert.AreEqual("SchedTask1", s.Name);
			Assert.AreEqual("1", s.Settings.TaskProperties["dummy"]);
		}


	}
}
