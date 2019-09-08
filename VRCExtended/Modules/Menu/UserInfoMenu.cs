using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCMenuUtils;
using VRChat.UI;

using VRC.UI;
using VRC.Core;

using VRCExtended.VRChat;

using UnityEngine;

using VRCModLoader;

namespace VRCExtended.Modules.Menu
{
    internal class UserInfoMenu : IExtendedModule
    {
        public void Setup() { }
        public IEnumerator AsyncSetup()
        {
            // Wait for UI
            yield return VRCMenuUtilsAPI.WaitForInit();

            // Add events
            VRCMenuUtilsAPI.OnPageShown += page => ModManager.StartCoroutine(VRCMenuUtilsAPI_OnPageShown(page));

            // Last Login
            LastLogin = new VRCEUiText("lastLogin", new Vector2(-470f, -130f), "", VRCEUi.UserInfoScreen.transform);
            LastLogin.TextObject.fontSize -= 20;

            // Refresh Button
            Refresh = new VRCEUiButton("refresh", new Vector2(0f, 0f), "Refresh");
            Refresh.OnClick += () =>
            {
                if (string.IsNullOrEmpty(PageUserInfo.userIdOfLastUserPageInfoViewed))
                    return;
                string id = PageUserInfo.userIdOfLastUserPageInfoViewed;

                ApiCache.Invalidate<APIUser>(id);
                APIUser.FetchUser(id, user =>
                {
                    PageUserInfo pageUserInfo = VRCEUi.UserInfoScreen.GetComponent<PageUserInfo>();

                    if (pageUserInfo != null)
                        pageUserInfo.SetupUserInfo(user);
                },
                error =>
                    ExtendedLogger.LogError($"Failed to fetch user of id {id}: {error}"));
            };
            VRCMenuUtilsAPI.AddUserInfoButton(Refresh);
        }

        #region UI Items
        public static VRCEUiButton Refresh { get; private set; }
        public static VRCEUiText LastLogin { get; private set; }
        #endregion
        #region UI Coroutines
        private IEnumerator VRCMenuUtilsAPI_OnPageShown(VRCUiPage page)
        {
            if (page.GetType() == typeof(PageUserInfo))
            {
                // Clear unknown
                LastLogin.Text = "";
                Refresh.ButtonObject.interactable = true;

                // Wait for userId
                while (string.IsNullOrEmpty(PageUserInfo.userIdOfLastUserPageInfoViewed))
                    yield return null;

                // Check if current user
                if (PageUserInfo.userIdOfLastUserPageInfoViewed == VRCEPlayer.Instance.UniqueID)
                {
                    Refresh.ButtonObject.interactable = false;
                    yield break;
                }

                // Grab latest
                APIUser.FetchUser(PageUserInfo.userIdOfLastUserPageInfoViewed, user =>
                {
                    LastLogin.Text = "Last login: " + DateTime.Parse(user.last_login).ToString("dd.MM.yyyy hh:mm");
                },
                error =>
                    ExtendedLogger.LogError($"Failed to fetch user of id {PageUserInfo.userIdOfLastUserPageInfoViewed}: {error}"));
            }
        }
        #endregion
    }
}
