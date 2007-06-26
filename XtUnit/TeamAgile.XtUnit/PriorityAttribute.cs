using System.Diagnostics;
using System.Threading;
using TeamAgile.ApplicationBlocks.Interception;

namespace TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions
{
	public class PriorityAttribute:BaseProcessingAttribute
	{
		private readonly ThreadPriority priority;


		public PriorityAttribute (ThreadPriority priority)
		{
			this.priority = priority;
		}

		[DebuggerStepThrough]
		protected override void OnPreProcess (object sender,PreProcessEventArgs args)
		{
			
			ThreadPriority oldPriority = Thread.CurrentThread.Priority;
			this.SaveSettingForPostProcess ("ThreadPriority",oldPriority);

			OutputDebugMessage("Setting thread priority from " + oldPriority + " to " + priority);
			Thread.CurrentThread.Priority = priority;

		}


		[DebuggerStepThrough]
		protected override void OnPostProcess (object sender,PostProcessEventArgs args)
		{
			ThreadPriority originalPriority = 
				(ThreadPriority) this.GetSettingFromPreProcess("ThreadPriority");
			
			OutputDebugMessage("Setting thread priority back to " + originalPriority);
			Thread.CurrentThread.Priority = originalPriority;
		}

	}
}
