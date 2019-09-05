using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Modules.ModuleExtensions;
using VRCExtended.Config;

using VRCMenuUtils;

namespace VRCExtended.Modules.General
{
    internal class PortalManager : IExtendedModule
    {
        #region Portal Variables
        private static PortalInternal _allowedPortal;
        #endregion

        public void Setup()
        {
            try
            {
                typeof(PortalInternal).GetMethod("Enter", BindingFlags.Public | BindingFlags.Instance).Patch(
                    typeof(PortalManager).GetMethod("Enter", BindingFlags.Static | BindingFlags.NonPublic));
                ExtendedLogger.Log("Patches in PortalManager setup successfully!");
            }
            catch(Exception ex)
            {
                ExtendedLogger.LogError("Failed to setup patches in PortalManager!", ex);
            }
        }
        public IEnumerator AsyncSetup() { yield break; }

        #region Overrides
        private static bool Enter(PortalInternal __instance, MethodInfo __originalMethod)
        {
            if ((bool)ManagerConfig.Config.General.DisablePortals)
                return false;
            if (_allowedPortal == __instance)
                return true;

            if((bool)ManagerConfig.Config.General.AskUsePortal)
            {
                VRCMenuUtilsAPI.Alert("Enter Portal", "Do you really want to enter the portal?", "No", () =>
                {
                    VRCMenuUtilsAPI.HideCurrentPopup();
                }, "Yes", () =>
                {
                    _allowedPortal = __instance;
                    VRCMenuUtilsAPI.HideCurrentPopup();

                    __originalMethod.Invoke(__instance, new object[0]);
                });
                return false;
            }
            return true;
        }
        #endregion
    }
}
