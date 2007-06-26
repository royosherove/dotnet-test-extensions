using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace TeamAgile.ApplicationBlocks.Interception.Internal
{
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
			
			IMethodReturnMessage customReturnMessage = 
				mcm.LogicalCallContext.GetData("CustomReturnMessage") as IMethodReturnMessage;
			if(customReturnMessage!=null)
			{
				this.PostProcess(mcm,ref customReturnMessage);
				
				//do not execute anything else
				return customReturnMessage;
			}
			IMessage rtnMsg = nextSink.SyncProcessMessage(mcm);
			IMethodReturnMessage mrm = (rtnMsg as IMethodReturnMessage);
			this.PostProcess(mcm,ref mrm);
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
			ArrayList attrs = getMethodCallCustomAttributes(callMessage);
			ArrayList newCallMessages = new ArrayList();

			for(int i=0;i<attrs.Count;i++)
			{
				PreProcessEventArgs args = new PreProcessEventArgs(m_target,callMessage);
				((BaseProcessingAttribute)attrs[i]).PreProcess(this,args);
				//currently handles only a case where one attribute changes the 
				//value of an inside parameter
				if(args.HasNewParameterValues)
				{
					//this should contain a MEthodCallMEssageWrapper object with new parameter values
					newCallMessages.Add(args.MethodCallMessage);
				}
			}

			if(newCallMessages.Count==1)
			{
				callMessage = (IMethodCallMessage) newCallMessages[0];
			}
			if(newCallMessages.Count>1)
			{
				combineMultipleNewParamValuesIntoNewMessage (newCallMessages, ref callMessage);
			}

		}

		private static void combineMultipleNewParamValuesIntoNewMessage (ArrayList newCallMessages, ref IMethodCallMessage callMessage)
		{
			object[] combinedNewValues = new object[callMessage.Args.Length];
	
			foreach (IMethodCallMessage newCallMessage in  newCallMessages)
			{
				//notice that we run the rist that the same parameter can be changed twice
				//in two different places and the last change wins
				for (int i = 0; i < newCallMessage.Args.Length; i++)
				{
					combinedNewValues[i] = newCallMessage.Args[i];
				}	
			}
			MethodCallMessageWrapper newCombinedMessageCall = new MethodCallMessageWrapper(callMessage);
			newCombinedMessageCall.Args = combinedNewValues;
			callMessage = newCombinedMessageCall;
		}

		[DebuggerStepThrough]
		private  ArrayList getMethodCallCustomAttributes(IMethodCallMessage msg)
		{
			ArrayList attribList = new ArrayList();
			Type type = typeof(BaseProcessingAttribute);
			
			BaseProcessingAttribute[] attributes = (BaseProcessingAttribute[])
													msg.MethodBase.GetCustomAttributes(type,true);
			attribList.AddRange (attributes);
			
			foreach (ParameterInfo pinfo in msg.MethodBase.GetParameters())
			{
				attribList.AddRange (pinfo.GetCustomAttributes(type,true));
			}

			return attribList;
		}

		private  ICollection getMethodCallParamsCustomAttributes(IMethodCallMessage msg)
		{
			ArrayList attribList = new ArrayList();
			Type type = typeof(BaseProcessingAttribute);
				foreach (ParameterInfo pinfo in msg.MethodBase.GetParameters())
			{
				attribList.AddRange (pinfo.GetCustomAttributes(type,true));
			}
			
			return attribList;
		}

		[DebuggerStepThrough]
		private void PostProcess(IMethodCallMessage callMessage, ref IMethodReturnMessage returnMessage)
		{
			ArrayList attrs = getMethodCallCustomAttributes(callMessage);
			PostProcessEventArgs args = new PostProcessEventArgs( m_target,callMessage,returnMessage);
			
			for(int i=0;i<attrs.Count;i++)
			{
				((BaseProcessingAttribute)attrs[i])
							.PostProcess(this,args);
			}
			returnMessage = args.MethodCallReturnMessage;
		}

	}

}
