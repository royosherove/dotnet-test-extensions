using NUnit.Framework;
using TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions;

namespace TeamAgile.XtUnit.Samples
{
	[TestFixture]
	//note that we MUST inherit from ExtensibleFixture (or InterceptableObject) 
	//for the interception, and thus our samples, to work
	public class SampleTestFixture:ExtensibleFixture
	{
		[Test,DataRollBack]
		public void MyDataRelatedTest()
		{
			//this method will be performed inside a COM+ transaction
			//this requires windows XP SP2 or better
			//Windows Server 2003 works as well.
		}

		[Test,ShowTracing]
		public void TracedTest()
		{
		  //Before this method executes, a debug output will be written

		  //After this method executes, a debug output will be written
		}

		[Test,CrossThreaded]
		public void MyThreadedTest()
		{
		   //this method will be perfomed on a different thread
		}
	}
}
