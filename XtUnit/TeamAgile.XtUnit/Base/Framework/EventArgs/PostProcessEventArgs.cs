using System;
using System.Runtime.Remoting.Messaging;

namespace TeamAgile.ApplicationBlocks.Interception
{
	public class PostProcessEventArgs:PreProcessEventArgs
	{
		private IMethodReturnMessage methodCallReturnMessage;
		
		public IMethodReturnMessage MethodCallReturnMessage
		{
			get { return methodCallReturnMessage; }
			set { methodCallReturnMessage = value; }
		}

		public PostProcessEventArgs (MarshalByRefObject targetObject, IMethodCallMessage methodCallMessage,IMethodReturnMessage methodCallReturnMessage)
			:base(targetObject,methodCallMessage)
		{
			this.methodCallReturnMessage = methodCallReturnMessage;
		}

		public void SetNewReturnValue (object newReturnValue)
		{
			IMethodReturnMessage message = this.methodCallReturnMessage;
			if(message==null)
			{
				return ;
			}

			ReturnMessage newReturnMessage = new ReturnMessage(newReturnValue,message.OutArgs,message.OutArgCount,message.LogicalCallContext,this.MethodCallMessage);
			this.MethodCallReturnMessage = newReturnMessage;
		}
	}

}
