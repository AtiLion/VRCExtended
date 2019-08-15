using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCMenuUtils;
using VRChat.UI;
using VRChat.UI.QuickMenuUI;

using VRCModLoader;

using VRCExtended.UI.Components;

namespace VRCExtended.UI
{
    internal static class UIConfig
    {
        #region UI Variables
        private static bool _initialized = false;
        #endregion

        #region VRCTools Variables
        private static Transform _settingsMenu;
        private static Transform _btnVRCToolsSettings;
        #endregion

        #region External UIs
        public static VRCEUiPage ConfigPage { get; private set; }
        public static ScrollviewConfig ConfigScroll { get; private set; }

        public static VRCEUiQuickButton ConfigButton { get; private set; }
        #endregion

        #region UI Functions
        public static IEnumerator Setup()
        {
            if (_initialized)
                yield break;

            // Wait for setup
            yield return VRCMenuUtilsAPI.WaitForInit();

            // Add config button
            if(ModManager.Mods.Any(a => a.Name == "VRCTools" && a.Author == "Slaynash"))
            {
                // Wait for VRCTools UI
                ExtendedLogger.Log("Found VRCTools! Hooking into VRCTools UI...");
                yield return WaitForVRCToolsUI();

                // Add to settings
                Transform settingsMenu = VRCEUi.QuickMenu.transform.Find("SettingsMenu");
                if(settingsMenu != null)
                {
                    RectTransform rtModSettings = _btnVRCToolsSettings.GetComponent<RectTransform>();
                    Vector3 btnModSettingsPos = rtModSettings.localPosition;
                    Vector2 btnModSettingsSize = rtModSettings.sizeDelta;

                    ConfigButton = new VRCEUiQuickButton("VRCExtended Settings", new Vector2(btnModSettingsPos.x + btnModSettingsSize.x + 10f, btnModSettingsPos.y), "Extended\nSettings", "Configuration for VRCExtended", settingsMenu);
                }
            }
            else
            {
                ExtendedLogger.Log("No settings modifier found! Using VRCMenuUtils...");
                ConfigButton = new VRCEUiQuickButton("VRCExtended Settings", new Vector2(0f, 0f), "Extended\nSettings", "Configuration for VRCExtended");

                VRCMenuUtilsAPI.AddQuickMenuButton(ConfigButton);
            }

            // Setup config button
            ConfigButton.OnClick += () =>
            {
                if (ConfigPage == null)
                    return;

                VRCMenuUtilsAPI.ShowUIPage(ConfigPage.Page);
            };

            // Create config page
            ConfigPage = new VRCEUiPage("ExtendedConfig", "VRCExtended Configuration");
            ConfigScroll = new ScrollviewConfig("ExtendedConfigScroll", ConfigPage);
            UICategoryConfig uiCategory1 = new UICategoryConfig("category1", "Test category 1", ConfigScroll.ContentControl);
            UICategoryConfig uiCategory2 = new UICategoryConfig("category2", "Test category 2", ConfigScroll.ContentControl);

            // Finish
            _initialized = true;
        }
        #endregion

        #region Waiters
        private static IEnumerator WaitForVRCToolsUI()
        {
            if (!ModManager.Mods.Any(a => a.Name == "VRCTools" && a.Author == "Slaynash"))
                yield break;

            while ((_settingsMenu = VRCEUi.QuickMenu.transform.Find("SettingsMenu")) == null)
                yield return null;
            while ((_btnVRCToolsSettings = _settingsMenu.Find("Mod Settings")) == null)
                yield return null;
        }
        #endregion
    }
}
