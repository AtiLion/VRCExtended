using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC.Core;
using VRC.UI;

using UnityEngine;

using VRCMenuUtils;
using VRChat.UI;

using VRCModLoader;
using VRCTools;
using ModPrefs = VRCTools.ModPrefs;

using VRCExtended.Modules;
using VRCExtended.Config;
using VRCExtended.UI;
using VRCExtended.VRChat;

namespace VRCExtended
{
    /* NOTES:
     * Button.colors <- change UI color UwU
    */

    [VRCModInfo("VRCExtended", "1.0.0", "AtiLion", "https://github.com/AtiLion/VRCExtended/releases", "vrcextended")]
    internal class VRCExtended : VRCMod
    {
        #region Configuration Variables
        private List<MapConfig> _configWatch = new List<MapConfig>();
        private DateTime _configLastCheck;
        #endregion

        #region UI Properties
        public static VRCEUiButton UserInfo_Refresh { get; private set; }
        public static VRCEUiText UserInfo_LastLogin { get; private set; }
        #endregion

        #region VRCMod Functions
        void OnApplicationStart()
        {
            ExtendedLogger.Log("Loading VRCExtended...");

            // Load managers
            {
                // Load config
                ExtendedLogger.Log("Loading ManagerConfig...");
                ManagerConfig.Load();

                // Load modules
                ExtendedLogger.Log("Loading ManagerModule...");
                ManagerModule.Setup();
            }

            // Use VRCTools mod settings
            foreach (MapConfig map in ManagerConfig.Maps)
                LoadMapConfig(map);

            // Load UI
            ModManager.StartCoroutine(LoadUI());

            ExtendedLogger.Log("VRCExtended loaded!");
        }
        void OnFixedUpdate()
        {
            // Update configs
            if(_configLastCheck == null || (DateTime.Now - _configLastCheck).TotalMilliseconds >= 1000) // Update every second
            {
                bool save = false;
                foreach(MapConfig conf in _configWatch)
                {
                    bool reqBool = ModPrefs.GetBool(conf.Parent.GetType().Name, conf.Name);
                    if (reqBool != (bool)conf.Value)
                    {
                        ExtendedLogger.Log($"Found change in {conf.Name} of category {conf.Parent.GetType().Name} to {reqBool}");
                        conf.Value = reqBool;
                        save = true;
                    }
                }
                if (save)
                    ManagerConfig.Save();

                _configLastCheck = DateTime.Now;
            }
        }
        #endregion

        #region Configuration Functions
        void LoadMapConfig(MapConfig conf)
        {
            if(conf.MapType == EMapConfigType.CATEGORY)
            {
                ModPrefs.RegisterCategory(conf.Type.Name, conf.Name);
                foreach(MapConfig subConf in conf.Children)
                    LoadMapConfig(subConf);
            }
            else if(conf.Type == typeof(bool?))
            {
                ModPrefs.RegisterPrefBool(conf.Parent.GetType().Name, conf.Name, (bool)conf.Value, conf.Name, !conf.Visible);
                ModPrefs.SetBool(conf.Parent.GetType().Name, conf.Name, (bool)conf.Value);

                _configWatch.Add(conf);
            }
        }
        #endregion

        #region Coroutine Loaders
        private static IEnumerator LoadUI()
        {
            // Wait for VRCMenuUtils
            yield return VRCMenuUtilsAPI.WaitForInit();

            // Add events
            VRCMenuUtilsAPI.OnPageShown += VRCMenuUtilsAPI_OnPageShown;

            // UserInfo Last Login
            UserInfo_LastLogin = new VRCEUiText("lastLogin", new Vector2(-470f, -130f), "", VRCEUi.UserInfoScreen.transform);
            UserInfo_LastLogin.TextObject.fontSize -= 20;

            // UserInfo Refresh Button
            UserInfo_Refresh = new VRCEUiButton("refresh", new Vector2(0f, 0f), "Refresh");
            UserInfo_Refresh.OnClick += () =>
            {
                if (string.IsNullOrEmpty(PageUserInfo.userIdOfLastUserPageInfoViewed))
                    return;
                string id = PageUserInfo.userIdOfLastUserPageInfoViewed;

                ApiCache.Invalidate<APIUser>(id);
                APIUser.FetchUser(id, (APIUser user) =>
                {
                    PageUserInfo pageUserInfo = VRCEUi.UserInfoScreen.GetComponent<PageUserInfo>();

                    if (pageUserInfo != null)
                        pageUserInfo.SetupUserInfo(user);
                },
                (string error) =>
                {
                    ExtendedLogger.LogError($"Failed to fetch user of id {id}: {error}");
                });
            };
            VRCMenuUtilsAPI.AddUserInfoButton(UserInfo_Refresh);
#if DEBUG
            // Load config UI
            ExtendedLogger.Log("Loading UIConfig...");
            yield return UIConfig.Setup();
#endif

            // Finish
            ExtendedLogger.Log("VRCExtended UI loaded!");
        }
        #endregion

        #region UI Event Handlers
        private static void VRCMenuUtilsAPI_OnPageShown(VRCUiPage page)
        {
            if(page.GetType() == typeof(PageUserInfo))
            {
                // Clear unknown
                UserInfo_LastLogin.Text = "";

                // Check if current user
                if (PageUserInfo.userIdOfLastUserPageInfoViewed == VRCEPlayer.Instance.UniqueID)
                    return;

                // Grab latest
                APIUser.FetchUser(PageUserInfo.userIdOfLastUserPageInfoViewed, (APIUser user) =>
                {
                    UserInfo_LastLogin.Text = "Last login: " + DateTime.Parse(user.last_login).ToString("dd.MM.yyyy hh:mm");
                },
                (string error) =>
                {
                    ExtendedLogger.LogError($"Failed to fetch user of id {PageUserInfo.userIdOfLastUserPageInfoViewed}: {error}");
                });
            }
        }
        #endregion
    }
}
