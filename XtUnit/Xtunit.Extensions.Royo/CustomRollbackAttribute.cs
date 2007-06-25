/// Created by Roy Osherove, http://www.iserializable.com
/// ------------------------------------------------------
using System;
using System.Diagnostics;
using System.EnterpriseServices;
using XtUnit.Framework;

namespace XtUnit.Extensions.Royo
{


	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor |AttributeTargets.Field,Inherited=true)]
	public class CustomRollBackAttribute:TestProcessingAttributeBase
	{

		[DebuggerStepThrough]
		protected override void OnPreProcess()
		{
			try
			{
				ServiceConfig config = new ServiceConfig();
				config.Transaction=TransactionOption.RequiresNew;
				config.TrackingAppName="Application Unit Tests";
				config.TransactionDescription="Application Unit Tests Transaction";
				config.TransactionTimeout=10000;

				OutputDebugMessage("ENTERING transaction context on method: " + methodCallMessage.MethodBase.Name);
				
				ServiceDomain.Enter(config);
				OutputDebugMessage("ENTRED transaction context on method: " + methodCallMessage.MethodBase.Name);


			}
			catch(Exception e)
			{
				OutputDebugMessage("Could not enter into a new transaction:\n" + e.ToString());			    
			}
		}

		[DebuggerStepThrough]
		protected override void OnPostProcess()
		{
			try
			{
				if(ContextUtil.IsInTransaction)
				{
					OutputDebugMessage("LEAVING transaction context on method: " + methodCallMessage.MethodBase.Name);
					ContextUtil.SetAbort();
				}
				ServiceDomain.Leave();
			}
			catch(Exception e)
			{
				OutputDebugMessage("Could not leave an existing transaction:\n" + e.ToString());			    
			}
		}
	}

}
