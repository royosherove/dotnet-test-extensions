/// Learn more about the technique shown here by reading the following articles:
/// http://weblogs.asp.net/rosherove/articles/dbunittesting.aspx
/// http://weblogs.asp.net/rosherove/archive/2004/07/20/187863.aspx
/// written by Roy Osherove (www.ISerializable.com)
/// 

using System;
using System.Diagnostics;
using System.EnterpriseServices;
using TeamAgile.ApplicationBlocks.Interception;

namespace TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions
{


	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor |AttributeTargets.Field,Inherited=true)]
	public class DataRollBackAttribute:BaseProcessingAttribute
	{

		[DebuggerStepThrough]
		protected override void OnPreProcess(object sender,PreProcessEventArgs args)
		{
			try
			{
				ServiceConfig config = new ServiceConfig();
				config.Transaction=TransactionOption.RequiresNew;
				config.TrackingAppName="Application Unit Tests";
				config.TransactionDescription="Application Unit Tests Transaction";
				config.TransactionTimeout=10000;

				OutputDebugMessage("ENTERING transaction context on method: " + args.MethodCallMessage.MethodBase.Name);
				
				ServiceDomain.Enter(config);
				OutputDebugMessage("ENTRED transaction context on method: " + args.MethodCallMessage.MethodBase.Name);


			}
			catch(Exception e)
			{
				OutputDebugMessage("Could not enter into a new transaction:\n" + e.ToString());			    
			}
		}

		[DebuggerStepThrough]
		protected override void OnPostProcess(object sender,PostProcessEventArgs args)
		{
			try
			{
				if(ContextUtil.IsInTransaction)
				{
					OutputDebugMessage("LEAVING transaction context on method: " + args.MethodCallMessage.MethodBase.Name);
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
