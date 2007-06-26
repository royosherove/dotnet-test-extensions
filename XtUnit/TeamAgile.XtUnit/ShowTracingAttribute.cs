/// Created by Roy Osherove, http://www.iserializable.com
/// ------------------------------------------------------
using System;
using System.Diagnostics;
using TeamAgile.ApplicationBlocks.Interception;

namespace TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions
{
	
	[Serializable]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor |AttributeTargets.Field,Inherited=true)]
	public class ShowTracingAttribute:BaseProcessingAttribute
	{

		[DebuggerStepThrough]
		protected override void OnPreProcess(object sender,PreProcessEventArgs args)
		{
			OutputDebugMessage("ENTERING method: " + args.MethodCallMessage.MethodBase.Name);
		}

		[DebuggerStepThrough]
		protected override void OnPostProcess(object sender,PostProcessEventArgs args)
		{
			OutputDebugMessage("LEAVING method: " + args.MethodCallMessage.MethodBase.Name);
		}
	}

}
