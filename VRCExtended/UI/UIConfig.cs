using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using VRCMenuUtils;
using VRChat.UI;
using VRChat.UI.QuickMenuUI;

using VRCModLoader;
using VRCTools;

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
            ConfigButton.Button.onClick.AddListener(() =>
            {
                if (ConfigPage == null)
                    return;

                // Copied from VRCTools
                VRCUiManagerUtils.GetVRCUiManager().ShowUi(false, true);
                ModManager.StartCoroutine(QuickMenuUtils.PlaceUiAfterPause());
                VRCUiManagerUtils.GetVRCUiManager().ShowScreen(ConfigPage.Page);
            });

            // Create config page
            ConfigPage = new VRCEUiPage("ExtendedConfig", "VRCExtended Configuration");

            // Setup config page


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
