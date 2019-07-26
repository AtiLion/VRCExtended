using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRChat.UI;

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
            ExtendedUser eUser = ExtendedServer.Users.FirstOrDefault(a => a.UniqueID == SelectedAPI.id);

            VRCExtended.ToggleUserInfoMore(false);
            VRCEUi.InternalUserInfoScreen.Moderator.gameObject.SetActive(false);
            if (APIUser.CurrentUser.id == __instance.user.id)
            {
                VRCExtended.UserInfoLastLogin.Text.text = "";
                VRCExtended.UserInfoMore.Button.interactable = false;
            }
            else
            {
                APIUser.FetchUser(__instance.user.id, (APIUser user) =>
                {
                    if (string.IsNullOrEmpty(user.last_login))
                        return;
                    DateTime dt = DateTime.Parse(user.last_login);

                    if (ModPrefs.GetBool("vrcextended", "useDTFormat"))
                        VRCExtended.UserInfoLastLogin.Text.text = "Last login: " + dt.ToString("MM.dd.yyyy hh:mm tt");
                    else
                        VRCExtended.UserInfoLastLogin.Text.text = "Last login: " + dt.ToString("dd.MM.yyyy hh:mm");
                },
                (string error) =>
                {
                    ExtendedLogger.LogError(error);
                });
                VRCExtended.UserInfoMore.Button.interactable = true;

                if(eUser != null)
                {
                    VRCExtended.UserInfoColliderControl.Button.interactable = true;
                    VRCExtended.UserInfoColliderControl.Text.text = (eUser.HasColliders ? "Disable colliders" : "Enable colliders");
                }
                else
                {
                    VRCExtended.UserInfoColliderControl.Button.interactable = false;
                    VRCExtended.UserInfoColliderControl.Text.text = "Not in world!";
                }
            }
        }
    }
}
