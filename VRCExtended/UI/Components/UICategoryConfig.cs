using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRChat.UI;

namespace VRCExtended.UI.Components
{
    internal class UICategoryConfig
    {
        #region UI Properties
        public Transform Control { get; private set; }
        public Transform HeaderControl { get; private set; }
        public Transform ContentControl { get; private set; }
        
        public Transform HeaderTextControl { get; private set; }
        #endregion

        #region Control Properties
        public ContentSizeFitter Control_SizeFitterObject { get; private set; }
        public VerticalLayoutGroup Control_LayoutGroupObject { get; private set; }
        public Image Control_ImageObject { get; private set; }

        public Image Header_ImageObject { get; private set; }
        public HorizontalLayoutGroup Header_LayoutGroupObject { get; private set; }
        public LayoutElement Header_LayoutElementObject { get; private set; }
        public Button Header_ButtonObject { get; private set; }

        public Text Header_Text_TextObject { get; private set; }
        public LayoutElement Header_Text_LayoutElementObject { get; private set; }

        public Image Content_ImageObject { get; private set; }
        public LayoutElement Content_LayoutElement { get; private set; }
        #endregion

        #region Quick Access Properties
        public bool IsOpen
        {
            get => ContentControl.gameObject.activeSelf;
            set => ContentControl.gameObject.SetActive(value);
        }
        #endregion

        public UICategoryConfig(string name, string text, Transform content)
        {
            // Create gameobjects
            GameObject goControl = new GameObject(name, typeof(RectTransform));
            GameObject goHeader = new GameObject("Header", typeof(RectTransform));
            GameObject goHeaderText = new GameObject("Text", typeof(RectTransform));
            GameObject goContent = new GameObject("Content", typeof(RectTransform));

            // Create control properties
            Control_LayoutGroupObject = goControl.AddComponent<VerticalLayoutGroup>();
            Control_SizeFitterObject = goControl.AddComponent<ContentSizeFitter>();
            Control_ImageObject = goControl.AddComponent<Image>();
            Header_ImageObject = goHeader.AddComponent<Image>();
            Header_LayoutGroupObject = goHeader.AddComponent<HorizontalLayoutGroup>();
            Header_LayoutElementObject = goHeader.AddComponent<LayoutElement>();
            Header_ButtonObject = goHeader.AddComponent<Button>();
            Header_Text_TextObject = goHeaderText.AddComponent<Text>();
            Header_Text_LayoutElementObject = goHeaderText.AddComponent<LayoutElement>();
            Content_ImageObject = goContent.AddComponent<Image>();
            Content_LayoutElement = goContent.AddComponent<LayoutElement>();

            // Set UI properties
            Control = goControl.transform;
            HeaderControl = goHeader.transform;
            HeaderTextControl = goHeaderText.transform;
            ContentControl = goContent.transform;

            // Setup category
            Control.SetParent(content);
            Control.localScale = Vector3.one;
            Control.localRotation = Quaternion.identity;
            Control.localPosition = Vector3.zero;

            // Setup category image
            Control_ImageObject.color = Color.cyan;

            // Setup category layout group
            Control_LayoutGroupObject.padding = new RectOffset(0, 0, 0, 0);
            Control_LayoutGroupObject.spacing = 0f;
            Control_LayoutGroupObject.childAlignment = TextAnchor.UpperLeft;
            Control_LayoutGroupObject.childControlHeight = true;
            Control_LayoutGroupObject.childControlWidth = true;
            Control_LayoutGroupObject.childForceExpandHeight = false;
            Control_LayoutGroupObject.childForceExpandWidth = false;

            // Setup category size fitter
            Control_SizeFitterObject.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            Control_SizeFitterObject.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Setup header
            HeaderControl.SetParent(Control);
            HeaderControl.localScale = Vector3.one;
            HeaderControl.localRotation = Quaternion.identity;
            HeaderControl.localPosition = Vector3.zero;

            // Setup header image
            Header_ImageObject.color = new Color(0f, 0.48f, 0.66f);

            // Setup header layout group
            Header_LayoutGroupObject.padding = new RectOffset(0, 0, 0, 0);
            Header_LayoutGroupObject.spacing = 0f;
            Header_LayoutGroupObject.childAlignment = TextAnchor.UpperLeft;
            Header_LayoutGroupObject.childControlHeight = true;
            Header_LayoutGroupObject.childControlWidth = true;
            Header_LayoutGroupObject.childForceExpandHeight = false;
            Header_LayoutGroupObject.childForceExpandWidth = false;

            // Setup header layout element
            Header_LayoutElementObject.ignoreLayout = false;
            Header_LayoutElementObject.preferredWidth = 570f;
            Header_LayoutElementObject.layoutPriority = 1;

            // Setup header button
            Header_ButtonObject.onClick.AddListener(() => IsOpen = !IsOpen);

            // Setup header text
            HeaderTextControl.SetParent(HeaderControl);
            HeaderTextControl.localScale = Vector3.one;
            HeaderTextControl.localRotation = Quaternion.identity;
            HeaderTextControl.localPosition = Vector3.zero;

            // Setup header text text
            Header_Text_TextObject.text = text;
            Header_Text_TextObject.font = VRCEUi.QuickMenu.transform.Find("ShortcutMenu/BuildNumText").GetComponent<Text>().font;
            Header_Text_TextObject.color = Color.black;
            Header_Text_TextObject.fontSize = 22;
            Header_Text_TextObject.alignByGeometry = true;
            Header_Text_TextObject.alignment = TextAnchor.MiddleCenter;

            // Setup header text layout element
            Header_Text_LayoutElementObject.ignoreLayout = false;
            Header_Text_LayoutElementObject.preferredWidth = 547f;
            Header_Text_LayoutElementObject.preferredHeight = 30f;
            Header_Text_LayoutElementObject.flexibleWidth = 1f;
            Header_Text_LayoutElementObject.flexibleHeight = 0f;
            Header_Text_LayoutElementObject.layoutPriority = 1;

            // Setup content
            ContentControl.SetParent(Control);
            ContentControl.localScale = Vector3.one;
            ContentControl.localRotation = Quaternion.identity;
            ContentControl.localPosition = Vector3.zero;

            // Setup content image
            Content_ImageObject.color = new Color(0f, 0.66f, 0.66f);

            // Setup content layout element
            Content_LayoutElement.ignoreLayout = false;
            Content_LayoutElement.preferredWidth = 570f;
            Content_LayoutElement.preferredHeight = 150f;
            Content_LayoutElement.flexibleWidth = 1f;
            Content_LayoutElement.layoutPriority = 1;
        }
    }
}
