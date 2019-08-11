using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCMenuUtils;

using VRCModLoader;

using VRCExtended.Config;
using VRCExtended.UI;

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

            // Load UI
            ModManager.StartCoroutine(LoadUI());

            ExtendedLogger.Log("VRCExtended loaded!");
        }
        #endregion

        #region Coroutine Loaders
        private static IEnumerator LoadUI()
        {
            // Wait for VRCMenuUtils
            yield return VRCMenuUtilsAPI.WaitForInit();

            // Load config UI
            ExtendedLogger.Log("Loading UIConfig...");
            yield return UIConfig.Setup();

            // Finish
            ExtendedLogger.Log("VRCExtended UI loaded!");
        }
        #endregion
    }
}
