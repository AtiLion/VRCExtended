using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using VRCModLoader;
using VRCTools;
using VRChat;
using VRChat.UI;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using VRCExtended.GameScripts;
using VRCExtended.Patches;

using VRC.Core;
using VRC.UI;
using VRCSDK2;

using ModPrefs = VRCTools.ModPrefs;

namespace VRCExtended
{
    // Crasher: 150k polygons in a mesh, tons of particles, shader blacklisting
    // Note to self: Message system is in the game, Notification | Type: message or broadcast

    [VRCModInfo("VRCExtended", "0.0.4.2", "AtiLion")]
    internal class VRCExtended : VRCMod
    {
        #region VRCExtended Variables
        private bool _initialized = false;
        #endregion

        #region Module Variables
        private bool p_anticrash;
        private bool p_localcolliders;
        private bool p_multilocalcolliders;
        private bool p_selflocalcolliders;
        private bool p_targetHandColliders;
        private bool p_fakeColliders;
        private bool p_fakeCollidersOthers;
        private bool p_smartColliders;
        private bool p_ignoreInsideColliders;
        private bool p_unlimitedFPS;
        #endregion

#if DEBUG
        #region VRCExtended UI Variables
        public static VRCEUiVolumeControl volVoice;
        public static VRCEUiVolumeControl volAvatar;
        #endregion
#endif

        #region VRCExtended Properties
        public static GameObject ScriptObject { get; private set; }
        public static int FrameRate { get; private set; } = 90;
        #endregion

        #region UserInfo UI
        public static VRCEUiButton UserInfoMore { get; private set; }
        public static VRCEUiButton UserInfoRefresh { get; private set; }
        public static VRCEUiButton UserInfoColliderControl { get; private set; }
        public static VRCEUiText UserInfoLastLogin { get; private set; }
        #endregion

        #region VRCMod Functions
        void OnApplicationStart()
        {
            ExtendedLogger.Log("Loading VRCExtended...");

            // Setup config
            ModPrefs.RegisterCategory("vrcextended", "VRCExtended");

            // VRCExtended
            ModPrefs.RegisterPrefBool("vrcextended", "useDTFormat", false, "Use American format");
            ModPrefs.RegisterPrefBool("vrcextended", "fpsManagement", false, "FPS management");
            ModPrefs.RegisterPrefBool("vrcextended", "unlimitedFPS", false, "Unlimited FPS");

            // Exploits
            ModPrefs.RegisterPrefBool("vrcextended", "askUsePortal", true, "Ask to use portal");
            ModPrefs.RegisterPrefBool("vrcextended", "disablePortal", false, "Disable portals");
            ModPrefs.RegisterPrefBool("vrcextended", "antiCrasher", false, "Prevent crashers");
#if DEBUG
            ModPrefs.RegisterPrefBool("vrcextended", "avatarLimiter", false, "Avatar limiter"); // TODO
#else
            ModPrefs.RegisterPrefBool("vrcextended", "avatarLimiter", false, "Avatar limiter", true);
#endif

            // Avatar
            ModPrefs.RegisterPrefBool("vrcextended", "localColliders", false, "Local colliders");
            ModPrefs.RegisterPrefBool("vrcextended", "multiLocalColliders", false, "Others have local colliders");
            ModPrefs.RegisterPrefBool("vrcextended", "selfLocalColliders", true, "Others can touch you");
            ModPrefs.RegisterPrefBool("vrcextended", "targetHandColliders", true, "Target only hand colliders");
            ModPrefs.RegisterPrefBool("vrcextended", "fakeColliders", false, "Add fake colliders to self");
            ModPrefs.RegisterPrefBool("vrcextended", "fakeCollidersOthers", false, "Add fake colliders to others");
            ModPrefs.RegisterPrefBool("vrcextended", "ignoreInsideColliders", true, "Ignore pull colliders");
#if DEBUG
            ModPrefs.RegisterPrefBool("vrcextended", "smartColliders", false, "Use smart colliders"); // TODO
#else
            ModPrefs.RegisterPrefBool("vrcextended", "smartColliders", false, "Use smart colliders", true);
#endif

            // Players
#if DEBUG
            ModPrefs.RegisterPrefBool("vrcextended", "userSpecificVolume", false, "User specific volumes");
#else
            ModPrefs.RegisterPrefBool("vrcextended", "userSpecificVolume", false, "User specific volumes", true); // Can't get this shit to work
#endif

            // Grab the previous data
            p_anticrash = ModPrefs.GetBool("vrcextended", "antiCrasher");
            p_localcolliders = ModPrefs.GetBool("vrcextended", "localColliders");
            p_multilocalcolliders = ModPrefs.GetBool("vrcextended", "multiLocalColliders");
            p_selflocalcolliders = ModPrefs.GetBool("vrcextended", "selfLocalColliders");
            p_targetHandColliders = ModPrefs.GetBool("vrcextended", "targetHandColliders");
            p_fakeColliders = ModPrefs.GetBool("vrcextended", "fakeColliders");
            p_fakeCollidersOthers = ModPrefs.GetBool("vrcextended", "fakeCollidersOthers");
            p_smartColliders = ModPrefs.GetBool("vrcextended", "smartColliders");
            p_ignoreInsideColliders = ModPrefs.GetBool("vrcextended", "ignoreInsideColliders");
            p_unlimitedFPS = ModPrefs.GetBool("vrcextended", "unlimitedFPS");

            // Add scripts
            ScriptObject = new GameObject();
            ScriptObject.AddComponent<PauseDetection>();
            GameObject.DontDestroyOnLoad(ScriptObject);

            // Get AntiCrasher config
            if (File.Exists("antiCrash.json"))
            {
                try
                {
                    if (JsonConvert.DeserializeObject<AntiCrasherConfig>(File.ReadAllText("antiCrash.json")).CheckBackwardsCompatibility())
                    {
                        File.WriteAllText("antiCrash.json", JsonConvert.SerializeObject(AntiCrasherConfig.Instance, Formatting.Indented));
                        ExtendedLogger.Log("Saved AntiCrasher config!");
                    }
                    ExtendedLogger.Log("Loaded AntiCrasher config!");
                }
                catch (Exception ex)
                {
                    ExtendedLogger.LogError("Failed to read/parse AntiCrasher config! Using default values...", ex);

                    AntiCrasherConfig.CreateDefault();
                }
            }
            else
            {
                AntiCrasherConfig.CreateDefault();
                ExtendedLogger.Log("Loaded default AntiCrasher config!");
            }
            if (ModManager.Mods.Any(a => a.Name == "QuitFix" && a.Author == "Herp Derpinstine") && !File.Exists("antiCrash.json"))
            {
                File.WriteAllText("antiCrash.json", JsonConvert.SerializeObject(AntiCrasherConfig.Instance, Formatting.Indented));
                ExtendedLogger.Log("Saved AntiCrasher config!");
            }

            // Get VolumeControl config
            VolumeControl.Setup();

            // Run patches
            Patch_PortalInternal.Setup();
            Patch_PageUserInfo.Setup();

            // Run coroutines
            ModManager.StartCoroutine(WaitForUIManager());
            if (ModPrefs.GetBool("vrcextended", "antiCrasher") && AntiCrasherConfig.Instance.UseOnlineBlacklist == true)
                ModManager.StartCoroutine(LoadShaderBlacklist());

            ExtendedLogger.Log("Loaded VRCExtended!");
        }

        void OnLateUpdate()
        {
            if(p_anticrash != ModPrefs.GetBool("vrcextended", "antiCrasher"))
            {
                if(ModPrefs.GetBool("vrcextended", "antiCrasher"))
                {
                    foreach(ExtendedUser user in ExtendedServer.Users)
                    {
                        if (user == null || user.Avatar == null)
                            continue;

                        try
                        {
                            user.RemoveCrashShaders();
                            user.LimitParticles();
                            user.RemoveCrashMesh();
                        }
                        catch (Exception ex)
                        {
                            ExtendedLogger.LogError("Error enabeling anti-crasher for user " + user.APIUser.displayName, ex);
                        }
                        
                    }
                    ExtendedLogger.Log("Enabled anti crasher!");
                }
                else
                {
                    foreach (ExtendedUser user in ExtendedServer.Users)
                    {
                        if (user == null || user.Avatar == null || user.APIUser == null)
                            continue;

                        try
                        {
                            user.RestoreCrashShaders();
                            user.RestoreParticleLimits();
                            user.RestoreCrashMesh();
                        }
                        catch (Exception ex)
                        {
                            ExtendedLogger.LogError("Error disabling anti-crasher for user " + user.APIUser.displayName, ex);
                        }
                    }
                    ExtendedLogger.Log("Disabled anti crasher!");
                }

                p_anticrash = ModPrefs.GetBool("vrcextended", "antiCrasher");
            }
            else if(p_localcolliders != ModPrefs.GetBool("vrcextended", "localColliders") ||
                    p_multilocalcolliders != ModPrefs.GetBool("vrcextended", "multiLocalColliders") ||
                    p_selflocalcolliders != ModPrefs.GetBool("vrcextended", "selfLocalColliders") ||
                    p_fakeColliders != ModPrefs.GetBool("vrcextended", "fakeColliders") ||
                    p_fakeCollidersOthers != ModPrefs.GetBool("vrcextended", "fakeCollidersOthers") ||
                    p_smartColliders != ModPrefs.GetBool("vrcextended", "smartColliders") ||
                    p_targetHandColliders != ModPrefs.GetBool("vrcextended", "targetHandColliders") ||
                    p_ignoreInsideColliders != ModPrefs.GetBool("vrcextended", "ignoreInsideColliders"))
            {
                // Clear colliders
                foreach (ExtendedUser user in ExtendedServer.Users)
                    if (user != null && user.Avatar != null)
                        user.RemoveLocalColliders();
                // Add them back settings based
                foreach (ExtendedUser user in ExtendedServer.Users)
                    if (user != null && user.Avatar != null)
                        user.OnAvatarCreated();

                p_localcolliders = ModPrefs.GetBool("vrcextended", "localColliders");
                p_multilocalcolliders = ModPrefs.GetBool("vrcextended", "multiLocalColliders");
                p_selflocalcolliders = ModPrefs.GetBool("vrcextended", "selfLocalColliders");
                p_fakeColliders = ModPrefs.GetBool("vrcextended", "fakeColliders");
                p_fakeCollidersOthers = ModPrefs.GetBool("vrcextended", "fakeCollidersOthers");
                p_smartColliders = ModPrefs.GetBool("vrcextended", "smartColliders");
                p_targetHandColliders = ModPrefs.GetBool("vrcextended", "targetHandColliders");
                p_ignoreInsideColliders = ModPrefs.GetBool("vrcextended", "ignoreInsideColliders");
                ExtendedLogger.Log("Reloaded local colliders!");
            }
            else if(p_unlimitedFPS != ModPrefs.GetBool("vrcextended", "unlimitedFPS"))
            {
                if (ModPrefs.GetBool("vrcextended", "unlimitedFPS"))
                    Application.targetFrameRate = 0;
                else
                    Application.targetFrameRate = FrameRate;
                p_unlimitedFPS = ModPrefs.GetBool("vrcextended", "unlimitedFPS");
            }
        }

        void OnLevelWasLoaded(int level) // Level 0 = Loading Screen, Level 1 = Login Screen, Level -1 = Game
        {
            if (level == 0)
                return;

            if(level == 1 && !_initialized)
            {
                // Setup FPS manager
                FrameRate = Application.targetFrameRate;
                ExtendedLogger.Log("Captured default FPS: " + FrameRate);
                if (ModPrefs.GetBool("vrcextended", "unlimitedFPS"))
                    Application.targetFrameRate = 0;

                // Setup systems
                VRCPlayerManager.Setup();
                VRCEPlayer.Setup();

                VRCPlayerManager.OnPlayerJoined += delegate (VRCEPlayer player)
                {
                    ExtendedServer.Users.Add(new ExtendedUser(player));
                    ExtendedLogger.Log("Player joined: " + player.APIUser.displayName);
                };
                VRCPlayerManager.OnPlayerLeft += delegate (VRCEPlayer player)
                {
                    ExtendedServer.Users.Remove(new ExtendedUser(player));
                    ExtendedLogger.Log("Player left: " + player.APIUser.displayName);
                };

                _initialized = true;
                return;
            }
            ExtendedServer.Users.Clear();
        }

        void OnApplicationQuit()
        {
            if (ModManager.Mods.Count(a => a.Name == "QuitFix" && a.Author == "Herp Derpinstine") > 1)
                return;
            if (!File.Exists("antiCrash.json"))
            {
                File.WriteAllText("antiCrash.json", JsonConvert.SerializeObject(AntiCrasherConfig.Instance, Formatting.Indented));
                ExtendedLogger.Log("Saved AntiCrasher config!");
            }
        }
#endregion

#region Coroutine Functions
        private IEnumerator LoadShaderBlacklist()
        {
            using(UnityWebRequest request = UnityWebRequest.Get("https://raw.githubusercontent.com/AtiLion/VRCExtended/master/ShaderBlacklist.txt"))
            {
                yield return request.SendWebRequest();

                if(!request.isNetworkError)
                {
                    List<string> blockedShaders = new List<string>();

                    blockedShaders.AddRange(AntiCrasherConfig.Instance.BlacklistedShaders);
                    foreach(string shader in request.downloadHandler.text.Split('\n'))
                        if (!string.IsNullOrEmpty(shader) && !blockedShaders.Contains(shader))
                            blockedShaders.Add(shader);
                    AntiCrasherConfig.Instance.BlacklistedShaders = blockedShaders.ToArray();
                    ExtendedLogger.Log("Downloaded shader blacklist!");
                }
                else
                    ExtendedLogger.LogError("Failed to get shader blacklist! " + request.error);
            }
        }
        private IEnumerator WaitForUIManager()
        {
            yield return VRCUiManagerUtils.WaitForUiManagerInit();

            // Load modules
#if DEBUG
            AddUserSpecificVolume();
#endif
            AddUserInfoButtons();
            AddSocialRefresh();

            // Debug
            /*Transform target = VRCEUi.InternalUserInfoScreen.UserPanel;
            ExtendedLogger.Log("Transform: " + target.name);
            foreach (Component component in target.GetComponents<Component>())
                ExtendedLogger.Log(" - " + component);
            for (int i = 0; i < target.childCount; i++)
            {
                ExtendedLogger.Log("Transform: " + target.GetChild(i).name);
                foreach (Component component in target.GetChild(i).GetComponents<Component>())
                    ExtendedLogger.Log(" - " + component);
            }*/
        }
        #endregion

        #region Module Functions
        public static void ToggleUserInfoMore(bool enabled)
        {
            Transform btnPlaylists = VRCEUi.InternalUserInfoScreen.PlaylistsButton;
            Transform btnFavorite = VRCEUi.InternalUserInfoScreen.FavoriteButton;
            Transform btnReport = VRCEUi.InternalUserInfoScreen.ReportButton;
            if (btnPlaylists == null || btnFavorite == null || btnReport == null)
            {
                ExtendedLogger.LogError("Failed to get required button!");
                return;
            }

            if (enabled)
            {
                UserInfoRefresh.Control.gameObject.SetActive(true);
                UserInfoColliderControl.Control.gameObject.SetActive(true);

                btnPlaylists.gameObject.SetActive(false);
                btnFavorite.gameObject.SetActive(false);
                btnReport.gameObject.SetActive(false);
                UserInfoMore.Text.text = "Less";
            }
            else
            {
                UserInfoRefresh.Control.gameObject.SetActive(false);
                UserInfoColliderControl.Control.gameObject.SetActive(false);

                btnPlaylists.gameObject.SetActive(true);
                btnFavorite.gameObject.SetActive(true);
                btnReport.gameObject.SetActive(true);
                UserInfoMore.Text.text = "More";
            }
        }
        #endregion

        #region Module Loading
#if DEBUG
        private void AddUserSpecificVolume()
        {
            if (!ModPrefs.GetBool("vrcextended", "userSpecificVolume"))
                return;
            if (VRCEUi.UserInfoScreen == null)
            {
                ExtendedLogger.LogError("Failed to find UserInfo screen!");
                return;
            }

            // -550 - 250f, 230f - 280f
            volVoice = new VRCEUiVolumeControl("volumeVoice", new Vector2(-620f, -230f), "Voice", 1f, VRCEUi.UserInfoScreen.transform);
            volAvatar = new VRCEUiVolumeControl("volumeAvatar", new Vector2(-290f, -230f), "Avatar", 1f, VRCEUi.UserInfoScreen.transform);
            if (!volVoice.Success || !volAvatar.Success)
            {
                ExtendedLogger.LogError("Failed to create volume sliders on UserInfo!");
                return;
            }

            // Setup volume events
            volVoice.Slider.onValueChanged.AddListener(delegate (float volume)
            {
                if (Patch_PageUserInfo.SelectedAPI == null)
                    return;
                ExtendedUser user = ExtendedServer.Users.FirstOrDefault(a => a.APIUser.id == Patch_PageUserInfo.SelectedAPI.id);

                if (user == null)
                    return;
                user.VolumeVoice = volume;
            });
            /*volAvatar.Slider.onValueChanged.AddListener(delegate (float volume)
            {
                if (Patch_PageUserInfo.SelectedUser == null)
                    return;
                Patch_PageUserInfo.SelectedUser.VolumeAvatar = volume;
            });*/
        }
#endif
        private void AddUserInfoButtons()
        {
            if (VRCEUi.UserInfoScreen == null)
            {
                ExtendedLogger.LogError("Failed to find UserInfo screen!");
                return;
            }

            Transform btnPlaylists = VRCEUi.InternalUserInfoScreen.PlaylistsButton;
            Transform btnFavorite = VRCEUi.InternalUserInfoScreen.FavoriteButton;
            Transform btnReport = VRCEUi.InternalUserInfoScreen.ReportButton;
            if (btnPlaylists == null || btnFavorite == null || btnReport == null)
            {
                ExtendedLogger.LogError("Failed to get required button!");
                return;
            }
            Vector3 pos = btnPlaylists.GetComponent<RectTransform>().localPosition;

            UserInfoLastLogin = new VRCEUiText("LastLoginText", new Vector2(-470f, -130f), "", VRCEUi.UserInfoScreen.transform);
            UserInfoLastLogin.Text.fontSize -= 20;

            UserInfoMore = new VRCEUiButton("More", new Vector2(pos.x, pos.y + 75f), "More", VRCEUi.InternalUserInfoScreen.UserPanel);
            UserInfoMore.Button.onClick.AddListener(() =>
            {
                if (Patch_PageUserInfo.SelectedAPI == null)
                    return;
                ToggleUserInfoMore(UserInfoMore.Text.text == "More");
            });

            UserInfoColliderControl = new VRCEUiButton("ColliderControl", new Vector2(pos.x, pos.y - 75f), "Not in world!", VRCEUi.InternalUserInfoScreen.UserPanel);
            UserInfoColliderControl.Control.gameObject.SetActive(false);
            UserInfoColliderControl.Button.onClick.AddListener(() =>
            {
                if (Patch_PageUserInfo.SelectedAPI == null)
                    return;
                ExtendedUser user = ExtendedServer.Users.FirstOrDefault(a => a.APIUser.id == Patch_PageUserInfo.SelectedAPI.id);

                if (user == null)
                    return;
                user.HasColliders = !user.HasColliders;
                UserInfoColliderControl.Text.text = (user.HasColliders ? "Disable colliders" : "Enable colliders");
            });

            UserInfoRefresh = new VRCEUiButton("Refresh", new Vector2(pos.x, pos.y), "Refresh", VRCEUi.InternalUserInfoScreen.UserPanel);
            UserInfoRefresh.Control.gameObject.SetActive(false);
            UserInfoRefresh.Button.onClick.AddListener(() =>
            {
                if (Patch_PageUserInfo.SelectedAPI == null)
                    return;

                ApiCache.Invalidate<APIUser>(Patch_PageUserInfo.SelectedAPI.id);
                APIUser.FetchUser(Patch_PageUserInfo.SelectedAPI.id, (APIUser user) =>
                {
                    PageUserInfo pageUserInfo = VRCEUi.UserInfoScreen.GetComponent<PageUserInfo>();
                    if (pageUserInfo != null)
                        pageUserInfo.SetupUserInfo(user);
                },
                (string error) =>
                {
                    ExtendedLogger.LogError(error);
                });
            });
            ExtendedLogger.Log("Setup PageUserInfo!");
        }
        private void AddSocialRefresh()
        {
            if (VRCEUi.SocialScreen == null)
                return;
            Transform currentStatus = VRCEUi.SocialScreen.transform.Find("Current Status");
            Transform btnStatus = currentStatus.Find("StatusButton");
            RectTransform rt_btnStatus = btnStatus.GetComponent<RectTransform>();
            Transform icnStatus = currentStatus.Find("StatusIcon");
            RectTransform rt_icnStatus = icnStatus.GetComponent<RectTransform>();
            Transform txtStatus = currentStatus.Find("StatusText");
            RectTransform rt_txtStatus = txtStatus.GetComponent<RectTransform>();

            VRCEUiButton btnRefresh = new VRCEUiButton("Refresh", new Vector2(rt_btnStatus.localPosition.x - 20f, rt_btnStatus.localPosition.y), "Refresh", currentStatus);
            RectTransform rt_btnRefresh = btnRefresh.Control.GetComponent<RectTransform>();

            rt_btnStatus.localPosition += new Vector3(210f, 0f, 0f);
            rt_icnStatus.localPosition += new Vector3(210f, 0f, 0f);
            rt_txtStatus.localPosition += new Vector3(210f, 0f, 0f);
            rt_btnRefresh.sizeDelta -= new Vector2(5f, 10f);

            btnRefresh.Button.onClick.AddListener(() =>
            {
                UiUserList[] userLists = VRCEUi.SocialScreen.GetComponentsInChildren<UiUserList>(true);

                foreach(UiUserList userList in userLists)
                {
                    userList.ClearAll();
                    userList.RefreshData();
                    userList.Refresh();
                    userList.Refresh(); // Don't question it
                }
                ExtendedLogger.Log("Refreshed social lists!");
            });
        }
#endregion
    }
}
