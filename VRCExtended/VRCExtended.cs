using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCModLoader;

using VRCExtended.Config;

namespace VRCExtended
{
    /* NOTES:
     * ApiGroup.FetchGroupNames(ownerId, ApiGroup.World.value, success, fail) - Get worlds of user
     * ApiGroup.FetchGroupNames(ownerId, ApiGroup.Avatar.value, success, fail) - Get avatars of user[unconfirmed]
    */

    [VRCModInfo("VRCExtended", "1.0.0", "AtiLion", "https://github.com/AtiLion/VRCExtended/releases", "vrcextended")]
    internal class VRCExtended : VRCMod
    {
        #region VRCMod Functions
        void OnApplicationStart()
        {
            ExtendedLogger.Log("Loading VRCExtended...");

            // Load managers
            {
                // Load config
                ExtendedLogger.Log("Loading ManagerConfig...");
                ManagerConfig.Load();
            }

            ExtendedLogger.Log("VRCExtended loaded!");
        }
        #endregion
    }
}
