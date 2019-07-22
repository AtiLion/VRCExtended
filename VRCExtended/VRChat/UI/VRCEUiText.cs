using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCExtended;

namespace VRChat.UI
{
    public class VRCEUiText
    {
        #region VRChatExtended Properties
        public bool Success { get; private set; }
        #endregion

        #region UI Properties
        public Transform Control { get; private set; }
        public Transform TextControl { get; private set; }

        public RectTransform Position { get; private set; }
        #endregion

        #region Control Properties
        public Text Text { get; private set; }
        #endregion

        public VRCEUiText(string name, Vector2 position, string text, Transform parent = null)
        {
            // Get required information
            Transform orgControl = VRCEUi.InternalUserInfoScreen.UsernameText;
            if (orgControl == null)
            {
                ExtendedLogger.LogError("Could not find Username text!");
                Success = false;
                return;
            }

            // Duplicate object
            GameObject goControl = GameObject.Instantiate(orgControl.gameObject);
            if (goControl == null)
            {
                ExtendedLogger.LogError("Could not duplicate Favorite button!");
                Success = false;
                return;
            }

            // Set UI properties
            Control = goControl.transform;
            TextControl = Control.GetComponent<Text>().transform;

            // Remove components that may cause issues
            GameObject.DestroyImmediate(Control.GetComponent<RectTransform>());

            // Set control properties
            Text = TextControl.GetComponent<Text>();

            // Set required parts
            if (parent != null)
                Control.SetParent(parent);
            goControl.name = name;

            // Modify RectTransform
            Position = Control.GetComponent<RectTransform>();
            RectTransform tmpRT = orgControl.GetComponent<RectTransform>();

            Position.localScale = tmpRT.localScale;
            Position.anchoredPosition = tmpRT.anchoredPosition;
            Position.sizeDelta = tmpRT.sizeDelta;
            Position.localPosition = new Vector3(position.x, position.y, 0f);
            Position.localRotation = tmpRT.localRotation;

            // Change UI properties
            Text.text = text;

            // Finish
            Success = true;
        }
    }
}
