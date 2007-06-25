/// Created by Roy Osherove, http://www.iserializable.com
/// ------------------------------------------------------
using System;
using System.Diagnostics;
using XtUnit.Framework;

namespace XtUnit.Framework
{
	
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor |AttributeTargets.Field,Inherited=true)]
	public class CustomTracingAttribute:TestProcessingAttributeBase
	{

		[DebuggerStepThrough]
		protected override void OnPreProcess()
		{
			OutputDebugMessage("ENTERING method: " + methodCallMessage.MethodBase.Name);
		}

		[DebuggerStepThrough]
		protected override void OnPostProcess()
		{
			OutputDebugMessage("LEAVING method: " + methodCallMessage.MethodBase.Name);
		}
	}

}
