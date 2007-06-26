using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Osherove.ThreadTester.Events
{
    [ComVisible(true), HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class AutoResetEventEx : EventWaitHandleEx
    {
        // Methods
        public AutoResetEventEx(bool initialState)
            : base(initialState, EventResetMode.AutoReset)
        {
        }
    }
}
