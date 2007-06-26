/// this class's code is partially based on code from the following article on codeproject:
/// "Intercepting method calls in C#, an approach to AOSD"
/// By J4amieC 
/// http://www.codeproject.com/csharp/AspectIntercept.asp

using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

namespace TeamAgile.ApplicationBlocks.Interception.Internal
{
	[AttributeUsage(AttributeTargets.Class)]
	public class InterceptAttribute : ContextAttribute
	{
		public InterceptAttribute() : base("Intercept")
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


}

