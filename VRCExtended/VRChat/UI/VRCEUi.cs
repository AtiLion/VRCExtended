using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRC.UI;

using VRCExtended;

namespace VRChat.UI
{
    public static class VRCEUi
    {
        #region VRChat Menu Variables
        private static QuickMenu _quickMenu;

        private static GameObject _avatarScreen;
        private static GameObject _detailsScreen;
        private static GameObject _playlistsScreen;
        private static GameObject _socialScreen;
        private static GameObject _settingsScreen;
        private static GameObject _safetyScreen;
        private static GameObject _userInfoScreen;
        private static GameObject _worldsScreen;
        #endregion

        #region VRChat Menu Properties
        public static QuickMenu QuickMenu
        {
            get
            {
                if (_quickMenu == null)
                    _quickMenu = (QuickMenu)typeof(QuickMenu).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
                return _quickMenu;
            }
        }

        public static GameObject AvatarScreen
        {
            get
            {
                if(_avatarScreen == null)
                    _avatarScreen = GameObject.Find(QuickMenu.avatarScreenPath);
                return _avatarScreen;
            }
        }
        public static GameObject DetailsScreen
        {
            get
            {
                if (_detailsScreen == null)
                    _detailsScreen = GameObject.Find(QuickMenu.detailsScreenPath);
                return _detailsScreen;
            }
        }
        public static GameObject PlaylistsScreen
        {
            get
            {
                if (_playlistsScreen == null)
                    _playlistsScreen = GameObject.Find(QuickMenu.playlistsScreenPath);
                return _playlistsScreen;
            }
        }
        public static GameObject SocialScreen
        {
            get
            {
                if (_socialScreen == null)
                    _socialScreen = GameObject.Find(QuickMenu.socialScreenPath);
                return _socialScreen;
            }
        }
        public static GameObject SettingsScreen
        {
            get
            {
                if (_settingsScreen == null)
                    _settingsScreen = GameObject.Find(QuickMenu.settingsScreenPath);
                return _settingsScreen;
            }
        }
        public static GameObject SafetyScreen
        {
            get
            {
                if (_safetyScreen == null)
                    _safetyScreen = GameObject.Find(QuickMenu.safetyScreenPath);
                return _safetyScreen;
            }
        }
        public static GameObject UserInfoScreen
        {
            get
            {
                if (_userInfoScreen == null)
                    _userInfoScreen = GameObject.Find(QuickMenu.userInfoScreenPath);
                return _userInfoScreen;
            }
        }
        public static GameObject WorldsScreen
        {
            get
            {
                if (_worldsScreen == null)
                    _worldsScreen = GameObject.Find(QuickMenu.worldsScreenPath);
                return _worldsScreen;
            }
        }
        #endregion

        #region VRChat UI Functions
        public static Transform DuplicateButton(Transform button, string name, string text, Vector2 offset, Transform parent = null)
        {
            // Create new one
            GameObject goButton = GameObject.Instantiate(button.gameObject);
            if(goButton == null)
            {
                ExtendedLogger.LogError("Could not duplicate button!");
                return null;
            }

            // Get UI components
            Button objButton = goButton.GetComponentInChildren<Button>();
            Text objText = goButton.GetComponentInChildren<Text>();

            // Destroy broke components
            GameObject.DestroyImmediate(goButton.GetComponent<RectTransform>());

            // Set required parts
            if (parent != null)
                goButton.transform.SetParent(parent);
            goButton.name = name;

            // Modify RectTransform
            RectTransform rtOriginal = button.GetComponent<RectTransform>();
            RectTransform rtNew = goButton.GetComponent<RectTransform>();

            rtNew.localScale = rtOriginal.localScale;
            rtNew.anchoredPosition = rtOriginal.anchoredPosition;
            rtNew.sizeDelta = rtOriginal.sizeDelta;
            rtNew.localPosition = rtOriginal.localPosition + new Vector3(offset.x, offset.y, 0f);
            rtNew.localRotation = rtOriginal.localRotation;

            // Change UI properties
            objText.text = text;
            objButton.onClick = new Button.ButtonClickedEvent();

            // Finish
            return goButton.transform;
        }
        #endregion

        #region VRChat Menu Screen Classes
        public static class InternalUserInfoScreen
        {
            #region UserInfo Variables
            private static PageUserInfo _instance;
            #endregion

            #region UserInfo UI Variables
            private static Transform _userPanel;
            private static Transform _avatarImage;
            #endregion

            #region UserPanel Variables
            private static Transform _playlistsButton;
            private static Transform _favoriteButton;
            #endregion

            #region UserInfo Properties
            public static PageUserInfo Instance
            {
                get
                {
                    if(_instance == null)
                    {
                        if (UserInfoScreen == null)
                            return null;
                        _instance = UserInfoScreen.GetComponent<VRCUiPage>() as PageUserInfo;
                    }
                    return _instance;
                }
            }
            #endregion

            #region UserInfo UI Properties
            public static Transform UserPanel
            {
                get
                {
                    if(_userPanel == null)
                    {
                        if (UserInfoScreen == null)
                            return null;
                        _userPanel = UserInfoScreen.transform.Find("User Panel");
                    }
                    return _userPanel;
                }
            }
            public static Transform AvatarImage
            {
                get
                {
                    if(_avatarImage == null)
                    {
                        if (UserInfoScreen == null)
                            return null;
                        _avatarImage = UserInfoScreen.transform.Find("AvatarImage");
                    }
                    return _avatarImage;
                }
            }
            #endregion

            #region UserPanel Properties
            public static Transform PlaylistsButton
            {
                get
                {
                    if(_playlistsButton == null)
                    {
                        if (UserPanel == null)
                            return null;
                        _playlistsButton = UserPanel.Find("Playlists");
                    }
                    return _playlistsButton;
                }
            }
            public static Transform FavoriteButton
            {
                get
                {
                    if (_favoriteButton == null)
                    {
                        if (UserPanel == null)
                            return null;
                        _favoriteButton = UserPanel.Find("Favorite");
                    }
                    return _favoriteButton;
                }
            }
            #endregion
        }
        public static class InternalSettingsScreen
        {
            #region Settings UI Variables
            private static Transform _volumePanel;
            #endregion

            #region VolumePanel Variables
            private static Transform _volumeMaster;
            #endregion

            #region Settings UI Properties
            public static Transform VolumePanel
            {
                get
                {
                    if (_volumePanel == null)
                    {
                        if (SettingsScreen == null)
                            return null;
                        _volumePanel = SettingsScreen.transform.Find("VolumePanel");
                    }
                    return _volumePanel;
                }
            }
            #endregion

            #region VolumePanel Properties
            public static Transform VolumeMaster
            {
                get
                {
                    if(_volumeMaster == null)
                    {
                        if (VolumePanel == null)
                            return null;
                        _volumeMaster = VolumePanel.Find("VolumeMaster");
                    }
                    return _volumeMaster;
                }
            }
            #endregion
        }
        #endregion
    }
}
