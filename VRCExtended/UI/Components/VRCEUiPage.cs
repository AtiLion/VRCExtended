using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRChat.UI;

namespace VRCExtended.UI.Components
{
    internal class VRCEUiPage
    {
        #region VRCUI Properties
        public bool Success { get; private set; }
        #endregion

        #region UI Properties
        public Transform Control { get; private set; }
        
        public RectTransform Position { get; private set; }
        #endregion

        #region Control Properties
        public VRCUiPage Page { get; private set; }
        #endregion

        #region Control Events
        public event Action OnPageActivated;
        public event Action OnpageDeatvitvated;
        #endregion

        public VRCEUiPage(string name, string displayName)
        {
            // Get defaults
            GameObject avatarScreen = VRCEUi.AvatarScreen;
            VRCUiPage avatarPage = avatarScreen.GetComponent<VRCUiPage>();
            if(avatarScreen == null || avatarPage == null)
            {
                ExtendedLogger.LogError("Could not find avatar screen!");
                Success = false;
                return;
            }

            // Create GameObject
            GameObject goControl = new GameObject(name);

            // Get positions
            Position = goControl.GetOrAddComponent<RectTransform>();

            // Set UI properties
            Control = goControl.transform;

            // Set control properties
            Page = goControl.AddComponent<VRCUiPage>();

            // Set required parts
            Control.SetParent(avatarScreen.transform.parent);

            // Change UI properties
            Page.AudioHide = avatarPage.AudioHide;
            Page.AudioShow = avatarPage.AudioShow;
            Page.displayName = displayName;
            Page.screenType = avatarPage.screenType;

            // Add control events
            Page.onPageActivated += () => OnPageActivated?.Invoke();
            Page.onPageDeactivated += () => OnpageDeatvitvated?.Invoke();

            // Finish
            Success = true;
        }
    }
}
