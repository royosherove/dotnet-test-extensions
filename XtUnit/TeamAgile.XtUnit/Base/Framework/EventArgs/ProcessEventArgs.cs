using System;
using System.Runtime.Remoting.Messaging;

namespace TeamAgile.ApplicationBlocks.Interception
{
	public abstract class ProcessEventArgs:EventArgs
	{
		protected  IMethodCallMessage methodCallMessage;
		protected MarshalByRefObject targetObject;
			
		public IMethodCallMessage MethodCallMessage
		{
			get { return methodCallMessage; }
		}
			
		public MarshalByRefObject TargetObject
		{
			get { return targetObject; }
		}

	}
}
