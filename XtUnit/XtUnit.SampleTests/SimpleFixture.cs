/// Created by Roy Osherove, http://www.iserializable.com
/// ------------------------------------------------------
using System.Diagnostics;
using NUnit.Framework;
using XtUnit.Extensions.Royo;
using XtUnit.Framework;

namespace ExtensibleUnitTesting.UI
{
	[TestFixture]
	public class SimpleFixture:TestFixtureBase
	{
		[Test,CustomTracing]
		public void SomeMethodWithTracing()
		{
			Debug.WriteLine("Performing some database stuff...");

		}

		[Test,CustomRollBack,CustomTracing]
		public void SomeMethodWithRollback()
		{
			Debug.WriteLine("Performing some database stuff...");
		}

	}
}
