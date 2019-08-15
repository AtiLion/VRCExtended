using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRChat.UI;

namespace VRCExtended.UI.Components
{
    internal class ScrollviewConfig
    {
        #region UI Properties
        public Transform Control { get; private set; }
        public Transform ContentControl { get; private set; }

        public RectTransform ContentPosition { get; private set; }
        #endregion

        #region Control Properties
        public ScrollRect ScrollRectObject { get; private set; }
        public ContentSizeFitter SizeFitterObject { get; private set; }
        public VerticalLayoutGroup LayoutGroupObject { get; private set; }
        public Mask MaskObject { get; private set; }
        public Image ImageObject { get; private set; }
        #endregion

        public ScrollviewConfig(string name, VRCEUiPage page)
        {
            // Create gameobjects
            GameObject goControl = new GameObject(name);
            GameObject goContent = new GameObject("Content");

            // Get positions
            RectTransform position = goControl.GetOrAddComponent<RectTransform>();
            ContentPosition = goContent.GetOrAddComponent<RectTransform>();

            // Create control properties
            ScrollRectObject = goControl.AddComponent<ScrollRect>();
            LayoutGroupObject = goContent.AddComponent<VerticalLayoutGroup>();
            SizeFitterObject = goContent.AddComponent<ContentSizeFitter>();
            ImageObject = goControl.AddComponent<Image>();
            MaskObject = goControl.AddComponent<Mask>();

            // Set UI properties
            Control = goControl.transform;
            ContentControl = goContent.transform;

            // Set required parts
            page.AddChild(Control);

            // Setup Scrollview
            Control.localScale = Vector3.one;
            Control.localRotation = Quaternion.identity;
            Control.localPosition = Vector3.zero;
            position.localPosition = new Vector3(0f, 0f, 0f);
            position.sizeDelta = new Vector2(1500f, 1000f);
            ScrollRectObject.vertical = true;
            ScrollRectObject.horizontal = false;

            // Setup Mask
            MaskObject.showMaskGraphic = false;
            ImageObject.color = Color.black;

            // Setup Content
            ContentControl.SetParent(Control);
            ContentControl.localScale = Vector3.one;
            ContentControl.localRotation = Quaternion.identity;
            ContentControl.localPosition = Vector3.zero;
            ContentPosition.localPosition = new Vector3(0f, 0f, 0f);
            ContentPosition.sizeDelta = new Vector2(1500f, 1000f);
            ContentPosition.anchorMin = new Vector2(0.5f, 1f);
            ContentPosition.anchorMax = new Vector2(0.5f, 1f);
            ContentPosition.pivot = new Vector2(0.5f, 1f);

            // Setup layout group
            LayoutGroupObject.padding = new RectOffset(2, 2, 2, 2);
            LayoutGroupObject.spacing = 5f;
            LayoutGroupObject.childAlignment = TextAnchor.UpperLeft;
            LayoutGroupObject.childControlHeight = true;
            LayoutGroupObject.childControlWidth = true;
            LayoutGroupObject.childForceExpandHeight = false;
            LayoutGroupObject.childForceExpandWidth = false;

            // Setup size fitter
            SizeFitterObject.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            SizeFitterObject.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRectObject.content = ContentPosition;
            ScrollRectObject.viewport = position;
        }
    }
}
