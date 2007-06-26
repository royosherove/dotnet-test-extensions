using System;
using System.Runtime.Remoting.Messaging;

namespace TeamAgile.ApplicationBlocks.Interception
{
	public class PreProcessEventArgs:ProcessEventArgs
	{
		private object[] parameterValuesCopy = null;
		private bool hasNewParameterValues = false;
		public bool HasNewParameterValues
		{
			get { return hasNewParameterValues; }
		}

		public PreProcessEventArgs (MarshalByRefObject targetObject, IMethodCallMessage methodCallMessage)
		{
			this.targetObject = targetObject;
			this.methodCallMessage = methodCallMessage;
			//automatically gets  us a copy of the array.
			parameterValuesCopy = methodCallMessage.Args;
		}

		public void SetNewArgValue (int parameterIndex, object paramValue)
		{
			parameterValuesCopy[parameterIndex] = paramValue;
			MethodCallMessageWrapper wrapper = new MethodCallMessageWrapper(this.methodCallMessage);
			wrapper.Args= parameterValuesCopy;

			this.methodCallMessage = wrapper;
			hasNewParameterValues=true;
		}
	}


	

}
