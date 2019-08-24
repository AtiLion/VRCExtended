using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRChat.UI;

namespace VRCExtended.UI.Components
{
    internal class UIToggleConfig
    {
        #region UI Properties
        public Transform Control { get; private set; }
        public Transform KeyControl { get; private set; }
        public Transform ValueControl { get; private set; }

        public Transform KeyTextControl { get; private set; }
        //public Transform ValueTextControl { get; private set; }
        #endregion

        #region Control Properties
        public HorizontalLayoutGroup Control_LayoutGroupObject { get; private set; }
        public LayoutElement Control_LayoutElementObject { get; private set; }
        public Image Control_ImageObject { get; private set; }

        public Text Key_TextObject { get; private set; }
        //public Image Key_ImageObject { get; private set; }
        public LayoutElement Key_LayoutElementObject { get; private set; }
        public ContentSizeFitter Key_SizeFitterObject { get; private set; }

        //public Toggle Value_ToggleObject { get; private set; }
        //public Text Value_TextObject { get; private set; }
        public LayoutElement Value_LayoutElementObject { get; private set; }
        public Image Value_ImageObject { get; private set; }
        #endregion

        public UIToggleConfig(string name, string text, Font font, bool isEnabled, Transform category)
        {
            // Create gameobjects
            GameObject goControl = new GameObject(name, typeof(RectTransform));
            GameObject goKey = new GameObject("Key", typeof(RectTransform));
            GameObject goKeyText = new GameObject("KeyText", typeof(RectTransform));
            GameObject goValue = new GameObject("Value", typeof(RectTransform));
            //GameObject goValueText = new GameObject("ValueText");

            // Create control properties
            Control_LayoutGroupObject = goControl.AddComponent<HorizontalLayoutGroup>();
            Control_LayoutElementObject = goControl.AddComponent<LayoutElement>();
            Control_ImageObject = goControl.AddComponent<Image>();
            //Key_ImageObject = goKey.AddComponent<Image>();
            Key_LayoutElementObject = goKey.AddComponent<LayoutElement>();
            Key_TextObject = goKeyText.AddComponent<Text>();
            Key_SizeFitterObject = goKeyText.AddComponent<ContentSizeFitter>();
            Value_LayoutElementObject = goValue.AddComponent<LayoutElement>();
            //Value_ToggleObject = goValue.AddComponent<Toggle>();
            Value_ImageObject = goValue.AddComponent<Image>();
            //Value_TextObject = goValueText.AddComponent<Text>();
            

            // Set UI properties
            Control = goControl.transform;
            KeyControl = goKey.transform;
            KeyTextControl = goKeyText.transform;
            ValueControl = goValue.transform;
            //ValueTextControl = goValueText.transform;

            // Setup item
            Control.SetParent(category);
            Control.localScale = Vector3.one;
            Control.localRotation = Quaternion.identity;
            Control.localPosition = Vector3.zero;

            // Setup item layout element
            Control_LayoutElementObject.ignoreLayout = false;
            Control_LayoutElementObject.preferredWidth = 570f;
            Control_LayoutElementObject.preferredHeight = 20f;
            Control_LayoutElementObject.flexibleHeight = 0f;
            Control_LayoutElementObject.layoutPriority = 1;

            // Setup item layout group
            Control_LayoutGroupObject.padding = new RectOffset(0, 0, 0, 0);
            Control_LayoutGroupObject.spacing = 0f;
            Control_LayoutGroupObject.childAlignment = TextAnchor.MiddleLeft;
            Control_LayoutGroupObject.childControlHeight = true;
            Control_LayoutGroupObject.childControlWidth = true;
            Control_LayoutGroupObject.childForceExpandHeight = false;
            Control_LayoutGroupObject.childForceExpandWidth = false;

            // Setup key
            KeyControl.SetParent(Control);
            KeyControl.localScale = Vector3.one;
            KeyControl.localRotation = Quaternion.identity;
            KeyControl.localPosition = Vector3.zero;
            //Key_ImageObject.color = Color.red;

            // Setup key layout element
            Key_LayoutElementObject.ignoreLayout = false;
            Key_LayoutElementObject.layoutPriority = 1;

            // Setup key text
            KeyTextControl.SetParent(KeyControl);
            KeyTextControl.localScale = Vector3.one;
            KeyTextControl.localRotation = Quaternion.identity;
            KeyTextControl.localPosition = Vector3.zero;

            // Setup key text size fitter
            Key_SizeFitterObject.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            //Key_SizeFitterObject.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Setup key text text
            Key_TextObject.text = text;
            Key_TextObject.font = font;
            Key_TextObject.color = Color.black;
            Key_TextObject.fontSize = 18;
            Key_TextObject.alignByGeometry = true;
            Key_TextObject.alignment = TextAnchor.MiddleCenter;

            // Setup value
            ValueControl.SetParent(Control);
            ValueControl.localScale = Vector3.one;
            ValueControl.localRotation = Quaternion.identity;
            ValueControl.localPosition = Vector3.zero;
            Value_ImageObject.color = Color.blue;

            // Setup value layout element
            Value_LayoutElementObject.ignoreLayout = false;
            //Value_LayoutElementObject.preferredWidth = 200f;
            //Value_LayoutElementObject.preferredHeight = 20f;
            Value_LayoutElementObject.layoutPriority = 1;

            // Setup value toggle
            /*Value_ToggleObject.targetGraphic = Value_ImageObject;
            Value_ToggleObject.transition = Selectable.Transition.None;
            ColorBlock block = Value_ToggleObject.colors;
            block.normalColor = Color.cyan;

            // Setup value image
            Value_ImageObject.color = Color.cyan;

            // Setup value text
            ValueTextControl.SetParent(ValueControl);
            ValueTextControl.localScale = Vector3.one;
            ValueTextControl.localRotation = Quaternion.identity;
            ValueTextControl.localPosition = Vector3.zero;

            // Setup value text text
            Value_TextObject.text = (isEnabled ? "On" : "Off");
            Value_TextObject.font = font;
            Value_TextObject.color = Color.black;
            Value_TextObject.fontSize = 15;
            Value_TextObject.alignByGeometry = true;
            Value_TextObject.alignment = TextAnchor.MiddleCenter;*/
        }
    }
}
