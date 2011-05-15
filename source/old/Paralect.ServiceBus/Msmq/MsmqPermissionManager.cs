using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Security.Principal;
using System.Text;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqPermissionManager
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string LocalAdministratorsGroupName = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount)).ToString();
        private static readonly string LocalEveryoneGroupName = new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)).ToString();
        private static readonly string LocalAnonymousLogonName = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null).Translate(typeof(NTAccount)).ToString();

        /// <summary>
        /// Sets default permissions for queue.
        /// </summary>
        public static void SetPermissionsForQueue(string queue, string account, string configurationName)
        {
            var q = new MessageQueue(queue);

            try
            {
                q.SetPermissions(LocalAdministratorsGroupName, MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
                q.SetPermissions(LocalEveryoneGroupName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);
                q.SetPermissions(LocalAnonymousLogonName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);

                q.SetPermissions(account, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);
                q.SetPermissions(account, MessageQueueAccessRights.ReceiveMessage, AccessControlEntryType.Allow);
                q.SetPermissions(account, MessageQueueAccessRights.PeekMessage, AccessControlEntryType.Allow);
            }
            catch (Exception ex)
            {
                var message = String.Format("Access to MSMQ queue '{0}' is denied. Please set permission for this queue to be accessable for '{1}' account. Service Bus [{2}]", queue, account, configurationName);

                _logger.ErrorException(message, ex);
                throw new Exception(message, ex);
            }
        } 
    }
}
