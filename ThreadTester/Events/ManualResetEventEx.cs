using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Osherove.ThreadTester.Events
{
//   
    public class ManualResetEventEx : EventWaitHandleEx
    {
        public ManualResetEventEx(bool initialState)
            : base(initialState, EventResetMode.ManualReset)
        {
        }
    }
}
