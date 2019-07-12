using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCExtended;

namespace VRChat.UI
{
    public class VRCEUiVolumeControl
    {
        #region VRChatExtended Properties
        public bool Success { get; private set; }
        #endregion

        #region UI Properties
        public Transform VolumeControl { get; private set; }
        public Transform FillArea { get; private set; }
        public Transform SliderLabel { get; private set; }
        public Transform Label { get; private set; }

        public RectTransform Position { get; private set; }
        #endregion

        #region Control Properties
        public Text Text { get; private set; }
        public Slider Slider { get; private set; }
        #endregion

        public VRCEUiVolumeControl(string name, Vector2 position, string text, float value = 1f, Transform parent = null)
        {
            // Get required information
            Transform orgVolumeMaster = VRCEUi.InternalSettingsScreen.VolumeMaster;
            if(orgVolumeMaster == null)
            {
                ExtendedLogger.LogError("Could not find VolumeMaster!");
                Success = false;
                return;
            }

            // Duplicate object
            GameObject goVolumeControl = GameObject.Instantiate(orgVolumeMaster.gameObject);
            if (goVolumeControl == null)
            {
                ExtendedLogger.LogError("Could not duplicate VolumeMaster!");
                Success = false;
                return;
            }

            // Set UI properties
            VolumeControl = goVolumeControl.transform;
            FillArea = VolumeControl.Find("FillArea");
            SliderLabel = VolumeControl.Find("SliderLabel");
            Label = VolumeControl.Find("Label");

            // Remove components that may cause issues
            GameObject.DestroyImmediate(VolumeControl.GetComponent<UiSettingConfig>());
            GameObject.DestroyImmediate(VolumeControl.GetComponent<RectTransform>());

            // Set control properties
            Text = Label.GetComponent<Text>();
            Slider = VolumeControl.GetComponent<Slider>();

            // Set required parts
            if (parent != null)
                VolumeControl.SetParent(parent);
            goVolumeControl.name = name;

            // Modify RectTransform
            Position = VolumeControl.GetComponent<RectTransform>();
            RectTransform tmpRT = orgVolumeMaster.GetComponent<RectTransform>();

            Position.localScale = tmpRT.localScale;
            Position.anchoredPosition = tmpRT.anchoredPosition;
            Position.sizeDelta = tmpRT.sizeDelta;
            Position.localPosition = new Vector3(position.x, position.y, 0f);
            Position.localRotation = tmpRT.localRotation;
            Label.GetComponent<RectTransform>().localPosition = Label.GetComponent<RectTransform>().localPosition + new Vector3(50f, 0f, 0f);

            // Change UI properties
            Text.text = text;
            Slider.value = value;

            // Finish
            Success = true;
        }
    }
}
