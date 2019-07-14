using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC;
using VRC.UI;
using VRC.Core;

using Harmony;

using VRCTools;

namespace VRCExtended.Patches
{
    internal class Patch_PageUserInfo
    {
        private Dictionary<APIUser, ExtendedUser> _volume = new Dictionary<APIUser, ExtendedUser>();
        
        public static APIUser SelectedAPI { get; private set; }

        public static void Setup()
        {
            // Setup harmony instances
            HarmonyInstance iSetupUserInfo = HarmonyInstance.Create("vrcextended.pageuserinfo.setupuserinfo");

            // Patch
            try
            {
                iSetupUserInfo.Patch(typeof(PageUserInfo).GetMethods(BindingFlags.Public | BindingFlags.Instance).First(a => a.Name == "SetupUserInfo" && a.GetParameters().Length > 1), null,
                                     new HarmonyMethod(typeof(Patch_PageUserInfo).GetMethod("SetupUserInfo", BindingFlags.NonPublic | BindingFlags.Static)));
                ExtendedLogger.Log("Patched PageUserInfo.SetupUserInfo");
            }
            catch (Exception ex)
            {
                ExtendedLogger.LogError("Failed to patch PageUserInfo!", ex);
                return;
            }
        }

        private static void SetupUserInfo(PageUserInfo __instance)
        {
            SelectedAPI = __instance.user;

            if (APIUser.CurrentUser.id == __instance.user.id)
            {
                VRCExtended.UserInfoRefresh.Button.interactable = false;
                VRCExtended.UserInfoDropPortal.Button.interactable = false;
            }
            else
            {
                VRCExtended.UserInfoRefresh.Button.interactable = true;
                VRCExtended.UserInfoDropPortal.Button.interactable = true;
            }
        }
    }
}
