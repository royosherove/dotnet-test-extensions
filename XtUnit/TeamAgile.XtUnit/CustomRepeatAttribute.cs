using System;
using System.Diagnostics;
using TeamAgile.ApplicationBlocks.Interception;

namespace TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property,AllowMultiple=true,Inherited=true)]
	[Serializable]
	public class CustomRepeatAttribute:BaseProcessingAttribute
	{
		private readonly int howManyTimes;

		public CustomRepeatAttribute(int howManyTimes)
		{
			this.howManyTimes = howManyTimes;
		}

		[DebuggerStepThrough]
		protected override void OnPreProcess (object sender,PreProcessEventArgs args)
		{
			for (int i = 0; i < howManyTimes-1; i++)
			{
				OutputDebugMessage("Executing method #" + i);
				this.invokeDeclaringMethod(args);
			}
			OutputDebugMessage("Executing method #" + (howManyTimes-1));

		}

		[DebuggerStepThrough]
		protected override void OnPostProcess (object sender,PostProcessEventArgs args)
		{
		}
	}
}
