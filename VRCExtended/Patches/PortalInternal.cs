using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Harmony;

using VRCTools;

namespace VRCExtended.Patches
{
    internal class Patch_PortalInternal
    {
        #region Continuation Variables
        private static PortalInternal portalEnter = null;
        #endregion

        public static void Setup()
        {
            // Setup harmony instances
            HarmonyInstance iEnter = HarmonyInstance.Create("vrcextended.portalinternal.enter");

            // Patch
            try
            {
                iEnter.Patch(typeof(PortalInternal).GetMethod("Enter", BindingFlags.Public | BindingFlags.Instance),
                             new HarmonyMethod(typeof(Patch_PortalInternal).GetMethod("Enter", BindingFlags.Static | BindingFlags.NonPublic)));
                ExtendedLogger.Log("Patched PortalInternal.Enter");
            }
            catch (Exception ex)
            {
                ExtendedLogger.LogError("Failed to patch PortalInternal: " + ex);
                return;
            }
        }

        private static bool Enter(PortalInternal __instance, MethodInfo __originalMethod)
        {
            if (ModPrefs.GetBool("vrcextended", "disablePortal"))
                return false;
            if (portalEnter == __instance)
                return true;

            if (ModPrefs.GetBool("vrcextended", "askUsePortal"))
            {
                VRCUiPopupManagerUtils.ShowPopup("Enter Portal", "Do you really want to enter the portal?", "No", delegate ()
                {
                    VRCUiPopupManagerUtils.GetVRCUiPopupManager().HideCurrentPopup();
                }, "Yes", delegate ()
                {
                    portalEnter = __instance;
                    VRCUiPopupManagerUtils.GetVRCUiPopupManager().HideCurrentPopup();
                    __originalMethod.Invoke(__instance, new object[] { });
                });
                return false;
            }
            return true;
        }
    }
}
