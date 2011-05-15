using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Paralect.ServiceBus
{
    public static class MutexFactory
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static Mutex CreateMutexWithFullControlRights(String name, out Boolean createdNew)
        {
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            MutexSecurity mutexSecurity = new MutexSecurity();
            MutexAccessRule rule = new MutexAccessRule(securityIdentifier, MutexRights.FullControl, AccessControlType.Allow);
            mutexSecurity.AddAccessRule(rule);
            return new Mutex(false, name, out createdNew, mutexSecurity);
        }

        public static void LockByMutex(String name, Action action)
        {
            bool mutexIsNew = false;
            Mutex queueMutex = CreateMutexWithFullControlRights(name, out mutexIsNew);
            Boolean owned = false;

            try
            {
                while (!owned)
                {
                    try { owned = queueMutex.WaitOne(-1, false); }
                    // Our main resource (queue) supposed to be always in the valid state. 
                    // That is why we are ignoring AbandonedMutexException.
                    // http://msdn.microsoft.com/en-us/library/system.threading.abandonedmutexexception.aspx
                    catch (AbandonedMutexException ex) { }
                }

                action();
            }
            finally
            {
                if (owned)
                {
                    queueMutex.ReleaseMutex();
                    queueMutex.Close();
                    
                }
            }            
        }
    }
}
