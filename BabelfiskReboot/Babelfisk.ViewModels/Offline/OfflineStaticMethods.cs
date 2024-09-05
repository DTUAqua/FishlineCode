using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;

namespace Babelfisk.ViewModels.Offline
{
    public static class OfflineStaticMethods
    {
        public static Task SyncUsersAsync(IAppRegionManager regionManager)
        {
            return Task.Factory.StartNew(() =>
            {
                //Only synchronize users if online
                if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                    return;

                BusinessLogic.SecurityManager secMan = new BusinessLogic.SecurityManager();

                try
                {
                    List<Entities.SprattusSecurity.Users> lstUsers = secMan.GetUsers();

                    var set = BusinessLogic.Settings.Settings.Instance;

                    set.OfflineUsers.AssignUsers(lstUsers);
                    set.Save();
                }
                catch (Exception e)
                {
                    new Action(() =>
                    {
                        regionManager.ShowMessageBox("En uventet fejl opstod i OfflineStaticMethods.SyncUsersAsync. " + e.Message);
                    }).Dispatch();
                }
            });
        }
    }
}
