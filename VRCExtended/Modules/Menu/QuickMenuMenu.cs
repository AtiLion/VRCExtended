using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCMenuUtils;
using VRChat.UI;
using VRChat.UI.QuickMenuUI;

using VRC.Core;
using VRC.UI;

using UnityEngine;

using VRCExtended.VRChat;

namespace VRCExtended.Modules.Menu
{
    internal class QuickMenuMenu : IExtendedModule
    {
        public void Setup() { }
        public IEnumerator AsyncSetup()
        {
            // Wait for UI
            yield return VRCMenuUtilsAPI.WaitForInit();

            // Headlight
            Headlight = new VRCEUiQuickButton("headlight", new Vector2(0f, 0f), "Toggle HeadLight", "Enables/Disables a headlight on your avatar");
            Headlight.OnClick += Headlight_OnClick;
            VRCMenuUtilsAPI.AddQuickMenuButton(Headlight);

            // Show avatar creator
            ShowAvatarCreator = new VRCEUiQuickButton("showavatarcreator", new Vector2(0f, 0f), "Show Avatar Creator", "Shows the creator of the currently used avatar");
            ShowAvatarCreator.OnClick += () => ShowAvatarCreator_OnClick();
            VRCMenuUtilsAPI.AddQuickMenuButton(ShowAvatarCreator);
        }

        #region UI Items
        public static VRCEUiQuickButton Headlight;
        public static VRCEUiQuickButton ShowAvatarCreator;
        #endregion
        #region UI Event Handlers
        private void Headlight_OnClick()
        {
            // Avatar check
            if (VRCEPlayer.Instance.VRCPlayer.avatarGameObject == null || VRCEPlayer.Instance.Animator == null)
                return;

            // Get head
            Transform head = VRCEPlayer.Instance.Animator.GetBoneTransform(HumanBodyBones.Head);
            if (head == null)
                return;
            Light light;

            // Toggle headlight
            if((light = head.GetComponent<Light>()) == null)
            {
                // Add headlight
                light = head.gameObject.AddComponent<Light>();

                light.color = Color.white;
                light.type = LightType.Spot;
                light.shadows = LightShadows.Soft;
                light.intensity = 0.8f;

                ExtendedLogger.Log("Added headlight to user!");
            }
            else
            {
                // Remove headlight
                GameObject.Destroy(light);
                ExtendedLogger.Log("Removed headlight from user!");
            }
        }
        private void ShowAvatarCreator_OnClick()
        {
            if (APIUser.CurrentUser == null || string.IsNullOrEmpty(APIUser.CurrentUser.avatarId))
                return;
            PageUserInfo userInfo = VRCEUi.UserInfoScreen.GetComponent<PageUserInfo>();

            // Get avatar info
            new ApiAvatar()
            {
                id = APIUser.CurrentUser.avatarId
            }.Get(container => {
                ApiAvatar avatar = container.Model as ApiAvatar;
                if (avatar == null || string.IsNullOrEmpty(avatar.authorId))
                    return;

                APIUser.FetchUser(avatar.authorId, user =>
                {
                    if (user == null)
                        return;

                    VRCMenuUtilsAPI.ShowUIPage(userInfo, false);
                    userInfo.SetupUserInfo(user);
                }, error =>
                    ExtendedLogger.LogError($"Failed to fetch user of id {avatar.authorId}: {error}"));
                    
            }, container =>
                ExtendedLogger.LogError($"Failed to get avatar information! {container.Error}"));
        }
        #endregion
    }
}
