using System;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace TeamAgile.ApplicationBlocks.Interception.Internal
{
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
}
