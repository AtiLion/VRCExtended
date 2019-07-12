using System;
using System.IO;
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
    // Crasher: 1m polygons in a mesh, tons of particles, shader blacklisting
    // ApiCache: Potential way of refreshing users

    [VRCModInfo("VRCExtended", "0.0.1.0", "AtiLion")]
    internal class VRCExtended : VRCMod
    {
        #region VRCExtended Variables
        private bool _initialized = false;
        private bool _initSocial = false;
        #endregion

        #region VRCExtended UI Variables
        public static VRCEUiVolumeControl volVoice;
        public static VRCEUiVolumeControl volAvatar;
        #endregion

        #region VRCExtended UI Properties
        public static VRCEUiButton RefreshButton { get; private set; }
        #endregion

        #region VRCMod Functions
        void OnApplicationStart()
        {
            ExtendedLogger.Log("Loading VRCExtended...");

            // Setup config
            ModPrefs.RegisterCategory("vrcextended", "VRCExtended");

            // Exploits
            ModPrefs.RegisterPrefBool("vrcextended", "askUsePortal", true, "Ask to use portal");
#if DEBUG
            ModPrefs.RegisterPrefBool("vrcextended", "antiCrasher", false, "Prevent crashers"); // Use shader blacklisting and polygon limit
#else
            ModPrefs.RegisterPrefBool("vrcextended", "antiCrasher", false, "Prevent crashers", true);
#endif

            // Avatar
            ModPrefs.RegisterPrefBool("vrcextended", "localColliders", false, "Local colliders");
            ModPrefs.RegisterPrefBool("vrcextended", "multiLocalColliders", false, "Others have local colliders");
            ModPrefs.RegisterPrefBool("vrcextended", "selfLocalColliders", true, "Others can touch you");

            // Players
            ModPrefs.RegisterPrefBool("vrcextended", "userSpecificVolume", false, "User specific volumes", true); // Can't get this shit to work

            // Get AntiCrasher config
            if(File.Exists("antiCrash.json"))
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

            ExtendedLogger.Log("Loaded VRCExtended!");
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

                if (VRCEUi.UserInfoScreen == null)
                {
                    ExtendedLogger.LogError("Failed to find UserInfo screen!");
                    return;
                }

                // -550 - 250f, 230f - 280f
                if(ModPrefs.GetBool("vrcextended", "userSpecificVolume"))
                {
                    /*volVoice = new VRCEUiVolumeControl("volumeVoice", new Vector2(-620f, -230f), "Voice", 1f, VRCEUi.UserInfoScreen.transform);
                    volAvatar = new VRCEUiVolumeControl("volumeAvatar", new Vector2(-290f, -230f), "Avatar", 1f, VRCEUi.UserInfoScreen.transform);
                    if (!volVoice.Success || !volAvatar.Success)
                    {
                        ExtendedLogger.LogError("Failed to create volume sliders on UserInfo!");
                        return;
                    }

                    // Setup volume events
                    volVoice.Slider.onValueChanged.AddListener(delegate (float volume)
                    {
                        if (Patch_PageUserInfo.SelectedUser == null)
                            return;
                        Patch_PageUserInfo.SelectedUser.VolumeVoice = volume;
                        ExtendedLogger.Log("Volume: " + Patch_PageUserInfo.SelectedUser.Voice.volume);
                    });
                    volAvatar.Slider.onValueChanged.AddListener(delegate (float volume)
                    {
                        if (Patch_PageUserInfo.SelectedUser == null)
                            return;
                        Patch_PageUserInfo.SelectedUser.VolumeAvatar = volume;
                    });*/
                }
                /*Transform target = VRCEUi.UserInfoScreen.transform.Find("AvatarImage");
                ExtendedLogger.Log("Transform: " + target.name);
                foreach (Component component in target.GetComponents<Component>())
                    ExtendedLogger.Log(" - " + component);
                for(int i = 0; i < target.childCount; i++)
                {
                    ExtendedLogger.Log("Transform: " + target.GetChild(i).name);
                    foreach (Component component in target.GetChild(i).GetComponents<Component>())
                        ExtendedLogger.Log(" - " + component);
                }*/

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

                Transform btnPlaylists = VRCEUi.InternalUserInfoScreen.PlaylistsButton;
                if(btnPlaylists == null)
                {
                    ExtendedLogger.LogError("Failed to get Playlists button!");
                    return;
                }
                Vector3 pos = btnPlaylists.GetComponent<RectTransform>().localPosition;
                RefreshButton = new VRCEUiButton("Refresh", new Vector2(pos.x, pos.y + 75f), "Refresh", VRCEUi.InternalUserInfoScreen.UserPanel);
                RefreshButton.Button.onClick.AddListener(() =>
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
                ExtendedLogger.Log("Setup PageUserInfo refresh button!");

                if(VRCEUi.SocialScreen != null && false) // Doesn't function properly
                {
                    VRCUiManagerUtils.OnPageShown += (page) =>
                    {
                        if (_initSocial)
                            return;
                        if (page.GetType() != typeof(VRCUiPageSocial))
                            return;
                        UiVRCList[] lists = page.GetComponentsInChildren<UiVRCList>(true);

                        foreach(UiVRCList list in lists)
                        {
                            if (list.GetType() != typeof(UiUserList) || list.transform.name == "Search" || list.transform.name == "FriendRequests")
                                continue;
                            Transform expandButton = list.transform.Find("Button");
                            RectTransform rtExpand = expandButton.GetComponent<RectTransform>();
                            VRCEUiButton btnRefresh = new VRCEUiButton("Refresh", new Vector2(rtExpand.localPosition.x, rtExpand.localPosition.y), "Refresh", list.transform);
                            RectTransform rtRefresh = btnRefresh.Control.GetComponent<RectTransform>();
                            Transform title = list.transform.Find("Title");

                            if(title != null)
                            {
                                RectTransform rtTitle = title.GetComponent<RectTransform>();

                                rtTitle.localPosition += new Vector3(220f, 0f, 0f);
                            }
                            rtExpand.localPosition += new Vector3(220f, 0f, 0f);
                            rtRefresh.sizeDelta -= new Vector2(0f, 25f);

                            btnRefresh.Button.onClick.AddListener(() =>
                            {
                                ((UiUserList)list).Refresh();
                            });
                            /*ExtendedLogger.Log("---------------------------");
                            Transform target = list.transform;
                            ExtendedLogger.Log("Transform: " + target.name);
                            foreach (Component component in target.GetComponents<Component>())
                                ExtendedLogger.Log(" - " + component);
                            for(int i = 0; i < target.childCount; i++)
                            {
                                ExtendedLogger.Log("Transform: " + target.GetChild(i).name);
                                foreach (Component component in target.GetChild(i).GetComponents<Component>())
                                    ExtendedLogger.Log(" - " + component);
                            }*/
                        }
                        _initSocial = true;
                    };
                }

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
        }
#endregion
    }
}
