﻿using System;
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

using VRCExtended.Patches;

using VRC.Core;
using VRC.UI;

using ModPrefs = VRCTools.ModPrefs;

namespace VRCExtended
{
    // Crasher: 150k polygons in a mesh, tons of particles, shader blacklisting

    [VRCModInfo("VRCExtended", "0.0.1.0", "AtiLion")]
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
        #endregion

        #region VRCExtended UI Variables
        public static VRCEUiVolumeControl volVoice;
        public static VRCEUiVolumeControl volAvatar;
        #endregion

        #region UserInfo UI
        public static VRCEUiButton UserInfoMore { get; private set; }
        public static VRCEUiButton UserInfoRefresh { get; private set; }
        public static VRCEUiButton UserInfoDropPortal { get; private set; }
        #endregion

        #region VRCMod Functions
        void OnApplicationStart()
        {
            ExtendedLogger.Log("Loading VRCExtended...");

            // Setup config
            ModPrefs.RegisterCategory("vrcextended", "VRCExtended");

            // Exploits
            ModPrefs.RegisterPrefBool("vrcextended", "askUsePortal", true, "Ask to use portal");
            ModPrefs.RegisterPrefBool("vrcextended", "antiCrasher", false, "Prevent crashers");

            // Avatar
            ModPrefs.RegisterPrefBool("vrcextended", "localColliders", false, "Local colliders");
            ModPrefs.RegisterPrefBool("vrcextended", "multiLocalColliders", false, "Others have local colliders");
            ModPrefs.RegisterPrefBool("vrcextended", "selfLocalColliders", true, "Others can touch you");

            // Fixes
            ModPrefs.RegisterPrefBool("vrcextended", "instantQuit", false, "Instantly quit the game");

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

            // Get AntiCrasher config
            if (File.Exists("antiCrash.json"))
            {
                try
                {
                    JsonConvert.DeserializeObject<AntiCrasherConfig>(File.ReadAllText("antiCrash.json"));
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

            // Get VolumeControl config
            VolumeControl.Setup();

            // Run patches
            Patch_PortalInternal.Setup();
            Patch_PageUserInfo.Setup();

            // Run coroutines
            ModManager.StartCoroutine(WaitForUIManager());

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
                        user.RemoveCrashShaders();
                        user.LimitParticles();
                        user.RemoveCrashMesh();
                    }
                    ExtendedLogger.Log("Enabled anti crasher!");
                }
                else
                {
                    foreach (ExtendedUser user in ExtendedServer.Users)
                    {
                        user.RestoreCrashShaders();
                        user.RestoreParticleLimits();
                        user.RestoreCrashMesh();
                    }
                    ExtendedLogger.Log("Disabled anti crasher!");
                }

                p_anticrash = ModPrefs.GetBool("vrcextended", "antiCrasher");
            }
            else if(p_localcolliders != ModPrefs.GetBool("vrcextended", "localColliders") ||
                    p_multilocalcolliders != ModPrefs.GetBool("vrcextended", "multiLocalColliders") ||
                    p_selflocalcolliders != ModPrefs.GetBool("vrcextended", "selfLocalColliders"))
            {
                // Clear colliders
                foreach (ExtendedUser user in ExtendedServer.Users)
                    user.RemoveLocalColliders();
                // Add them back settings based
                foreach (ExtendedUser user in ExtendedServer.Users)
                    user.OnAvatarCreated();

                p_localcolliders = ModPrefs.GetBool("vrcextended", "localColliders");
                p_multilocalcolliders = ModPrefs.GetBool("vrcextended", "multiLocalColliders");
                p_selflocalcolliders = ModPrefs.GetBool("vrcextended", "selfLocalColliders");
                ExtendedLogger.Log("Reloaded local colliders!");
            }
        }

        void OnLevelWasLoaded(int level) // Level 0 = Loading Screen, Level 1 = Login Screen, Level -1 = Game
        {
            if (level == 0)
                return;

            if(level == 1 && !_initialized)
            {
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
            if (!File.Exists("antiCrash.json"))
            {
                File.WriteAllText("antiCrash.json", JsonConvert.SerializeObject(AntiCrasherConfig.Instance, Formatting.Indented));
                ExtendedLogger.Log("Saved AntiCrasher config!");
            }
            if (ModPrefs.GetBool("vrcextended", "instantQuit"))
                Process.GetCurrentProcess().Kill();
            else
                ModManager.StartCoroutine(WaitForMods());
        }
        #endregion

        #region Coroutine Functions
        private IEnumerator WaitForUIManager()
        {
            yield return VRCUiManagerUtils.WaitForUiManagerInit();

            // Load modules
            AddUserSpecificVolume();
            AddUserInfoRefresh();
            AddSocialRefresh();

            // Debug
            /*Transform target = VRCEUi.InternalUserInfoScreen.Moderator.Find("Mic Controls");
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
        private IEnumerator WaitForMods()
        {
            yield return false;
            Process.GetCurrentProcess().Kill();
        }
        #endregion

        #region Module Loading
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
        private void AddUserInfoRefresh()
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

            UserInfoMore = new VRCEUiButton("More", new Vector2(pos.x, pos.y + 75f), "More", VRCEUi.InternalUserInfoScreen.UserPanel);
            UserInfoMore.Button.onClick.AddListener(() =>
            {
                if (Patch_PageUserInfo.SelectedAPI == null)
                    return;
                if(UserInfoMore.Text.text == "More")
                {
                    UserInfoRefresh.Control.gameObject.SetActive(true);
                    UserInfoDropPortal.Control.gameObject.SetActive(true);

                    btnPlaylists.gameObject.SetActive(false);
                    btnFavorite.gameObject.SetActive(false);
                    btnReport.gameObject.SetActive(false);
                    UserInfoMore.Text.text = "Less";
                }
                else
                {
                    UserInfoRefresh.Control.gameObject.SetActive(false);
                    UserInfoDropPortal.Control.gameObject.SetActive(false);

                    btnPlaylists.gameObject.SetActive(true);
                    btnFavorite.gameObject.SetActive(true);
                    btnReport.gameObject.SetActive(true);
                    UserInfoMore.Text.text = "More";
                }
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

            UserInfoDropPortal = new VRCEUiButton("DropPortal", new Vector2(pos.x, pos.y - 75f), "Drop Portal", VRCEUi.InternalUserInfoScreen.UserPanel);
            UserInfoDropPortal.Control.gameObject.SetActive(false);
            UserInfoDropPortal.Button.onClick.AddListener(() =>
            {
                if (Patch_PageUserInfo.SelectedAPI == null)
                    return;
                if(string.IsNullOrEmpty(Patch_PageUserInfo.SelectedAPI.location))
                {
                    VRCUiPopupManagerUtils.GetVRCUiPopupManager().ShowAlert("Error", "Invalid join location!");
                    return;
                }
                if(Patch_PageUserInfo.SelectedAPI.location == "private")
                {
                    VRCUiPopupManagerUtils.GetVRCUiPopupManager().ShowAlert("Error", "The user is in a private world!");
                    return;
                }
                if (Patch_PageUserInfo.SelectedAPI.location == "local")
                {
                    VRCUiPopupManagerUtils.GetVRCUiPopupManager().ShowAlert("Error", "The user is in a local world!");
                    return;
                }
                string[] location = Patch_PageUserInfo.SelectedAPI.location.Split(':');
                API.Fetch<ApiWorld>(location[0], (ApiContainer worldContainer) => {
                    ApiWorld world = (ApiWorld)worldContainer.Model;
                    ApiWorldInstance instance = world.worldInstances.FirstOrDefault(inst => inst != null && inst.idOnly == location[1]);

                    if (instance.InstanceType != ApiWorldInstance.AccessType.Public && instance.InstanceType != ApiWorldInstance.AccessType.FriendsOfGuests)
                    {
                        VRCUiPopupManagerUtils.GetVRCUiPopupManager().ShowAlert("Error", "You are only allowed to drop portals to public and friends+ worlds.");
                        return;
                    }
                    PortalInternal.CreatePortal(world, instance, VRCPlayer.Instance.transform.position, VRCPlayer.Instance.transform.forward, true);
                },
                (ApiContainer container) => {
                    if (container != null)
                        ExtendedLogger.LogError("Error fetching world: " + container.Error);
                });
            });
            ExtendedLogger.Log("Setup PageUserInfo refresh button!");
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
                    userList.FetchAndRenderElementsForCurrentPage();
                    userList.RefreshData();
                }
                ExtendedLogger.Log("Refreshed social lists!");
            });
        }
        #endregion
    }
}
