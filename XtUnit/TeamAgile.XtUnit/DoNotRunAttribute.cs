using System.Diagnostics;
using TeamAgile.ApplicationBlocks.Interception;

namespace TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions
{
	public class DoNotRunAttribute:BaseProcessingAttribute
	{

		[DebuggerStepThrough]
		protected override void OnPreProcess (object sender,PreProcessEventArgs args)
		{
			this.FlagCurrentMethodToBeSkipped(args);
			OutputDebugMessage("Skipping method " + args.MethodCallMessage.MethodName);
		}


		[DebuggerStepThrough]
		protected override void OnPostProcess (object sender,PostProcessEventArgs args)
		{
			//throw new NotImplementedException ();
		}
	}
}
