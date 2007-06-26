using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using TeamAgile.ApplicationBlocks.Interception;

namespace TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions
{
	/// <summary>
	/// Summary description for CrossThreadedAttribute.
	/// </summary>
	public class CrossThreadedAttribute:BaseProcessingAttribute
	{
		private static Hashtable runningThreads = new Hashtable();

		[DebuggerStepThrough]
		protected override void OnPreProcess (object sender, PreProcessEventArgs args)
		{
			int threadID = AppDomain.GetCurrentThreadId();
			if(runningThreads[threadID]!=null 
				|| runningThreads.Count>0)
			{
				OutputDebugMessage("skipping method run from inside previous runner...");
				return ;
			}
			else
			{
				runningThreads[threadID]="some not null value";
			}

			OutputDebugMessage(threadID + ": Preparing to run method on separate thread and join it afterwards...");
			CrossThreadRunner runner  = new CrossThreadRunner(args.TargetObject,args.MethodCallMessage.MethodBase,args.MethodCallMessage.Args);
			runner.Run();
			OutputDebugMessage("Ran it on a separate thread...skipping actual method invocation on current thread");

			this.FlagCurrentMethodToBeSkipped(args);
		}

		[DebuggerStepThrough]
		protected override void OnPostProcess (object sender, PostProcessEventArgs args)
		{
			//throw new NotImplementedException ();
		}


		#region CrossThreadRunner class
		//this class code from "borrowed" from Peter Provost's blog:
		//http://www.peterprovost.org/archive/2004/11/03/2051.aspx
		public class CrossThreadRunner
					      
		{
			private readonly object[] parameters;
			private readonly object targetObject;
			private MethodBase targetMethod;
			private Exception lastException;
					    
			public CrossThreadRunner(object targetObject,MethodBase userDelegate,object[] parameters)
			         
			{
				this.targetMethod = userDelegate;
				this.targetObject = targetObject;
				this.parameters = parameters;
			}
		   
			public void Run()
		          
			{
				Thread t = new Thread(new ThreadStart(this.MultiThreadedWorker));
		   
				t.Start();
				t.Join();
		   
				if (lastException != null)
					ThrowExceptionPreservingStack(lastException);
			}
		   
			[ReflectionPermission(SecurityAction.Demand)]
			private void ThrowExceptionPreservingStack(Exception exception)
		          
			{
				FieldInfo remoteStackTraceString = 
					typeof (Exception).GetField("_remoteStackTraceString", 
					BindingFlags.Instance | BindingFlags.NonPublic);
				remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);
				throw exception;
			}
		     
			private void MultiThreadedWorker()
		            
			{
				try
		                
				{
					targetMethod.Invoke(targetObject,parameters);
				}
		                
				catch (Exception e)
		                
				{
					lastException = e;
				}
			}
	
		}

		#endregion
	}
}
