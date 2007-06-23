/// Created by Roy Osherove, http://www.iserializable.com
/// ------------------------------------------------------
/// this class's code is partially based on code from the following article on codeproject:
/// "Intercepting method calls in C#, an approach to AOSD"
/// By J4amieC 
/// http://www.codeproject.com/csharp/AspectIntercept.asp

using System;
using System.Diagnostics;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace XtUnit.Framework.Internal
{
	
	

	#region InterceptAttribute
	[AttributeUsage(AttributeTargets.Class)]
	public class InterceptAttribute : ContextAttribute
	{
		
		public InterceptAttribute() : base("Intercept")
		{
		}

		public override void Freeze(Context newContext)
		{			
		}

		public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
		{
			ctorMsg.ContextProperties.Add( new InterceptProperty() );
		}

		public override bool IsContextOK(Context ctx, IConstructionCallMessage ctorMsg)
		{
			InterceptProperty p = ctx.GetProperty("Intercept") as InterceptProperty;
			if(p == null)
				return false;
			return true;
		}

		public override bool IsNewContextOK(Context newCtx)
		{
			InterceptProperty p = newCtx.GetProperty("Intercept") as InterceptProperty;
			if(p == null)
				return false;
			return true;
		}

		
	}

#endregion //InterceptAttribute


	#region Attribute helpers and sinks

	
	//IContextProperty, IContributeServerContextSink
	 class InterceptProperty : IContextProperty, IContributeObjectSink
	{
		public InterceptProperty() : base()
		{
		}
		#region IContextProperty Members

		public string Name
		{
			get
			{
				return "Intercept";
			}
		}

		public bool IsNewContextOK(Context newCtx)
		{
			InterceptProperty p = newCtx.GetProperty("Intercept") as InterceptProperty;
			if(p == null)
				return false;
			return true;
		}

		public void Freeze(Context newContext)
		{
		}

		#endregion

		#region IContributeObjectSink Members

		public IMessageSink GetObjectSink(MarshalByRefObject target, IMessageSink nextSink)
		{
			return new InterceptSink(target, nextSink);
		}

		#endregion
	}

	 class InterceptSink : IMessageSink
	{
		private IMessageSink nextSink;
	 	private MarshalByRefObject m_target=null;

	 	[DebuggerStepThrough]
		public InterceptSink(MarshalByRefObject target, IMessageSink nextSink)
		{
			m_target = target;
			this.nextSink = nextSink;
		}

		#region IMessageSink Members

		[DebuggerStepThrough]
		public IMessage SyncProcessMessage(IMessage msg)
		{
			IMethodCallMessage mcm = (msg as IMethodCallMessage);
			this.PreProcess(ref mcm);
			IMessage rtnMsg = nextSink.SyncProcessMessage(msg);
			IMethodReturnMessage mrm = (rtnMsg as IMethodReturnMessage);
			this.PostProcess(msg as IMethodCallMessage,ref mrm);
			return mrm;
		}

		public IMessageSink NextSink
		{
			get
			{
				return this.nextSink;
			}
		}

		[DebuggerStepThrough]
		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			IMessageCtrl rtnMsgCtrl = nextSink.AsyncProcessMessage(msg,replySink);
			return rtnMsgCtrl;
		}

		#endregion
		
		[DebuggerStepThrough]
		private void PreProcess(ref IMethodCallMessage callMessage)
		{
			ProcessingAttributeBase[] attrs = getMethodCallCustomAttributes(callMessage);
			for(int i=0;i<attrs.Length;i++)
			{
			    attrs[i].DeclaringType = callMessage.MethodBase.DeclaringType;
			    attrs[i].DeclaringMethod = callMessage.MethodBase;
			    attrs[i].PreProcess(m_target,ref callMessage);
			}
		}


	    [DebuggerStepThrough]
	 	private static ProcessingAttributeBase[] getMethodCallCustomAttributes(IMethodCallMessage msg)
	 	{
	 		return (ProcessingAttributeBase[])msg.MethodBase.GetCustomAttributes(typeof(ProcessingAttributeBase),true);
	 	}

		[DebuggerStepThrough]
	 	private void PostProcess(IMethodCallMessage callMessage, ref IMethodReturnMessage returnMessage)
		{
			ProcessingAttributeBase[] attrs = getMethodCallCustomAttributes(callMessage);
			for(int i=attrs.Length-1;i>-1;i--)
				attrs[i].PostProcess(m_target,ref callMessage,ref returnMessage);			
		}

	}

	
#endregion //Attribute helpers and sinks

}
