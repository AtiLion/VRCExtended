using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC.Core;
using VRC.UI;

using UnityEngine;
using UnityEngine.Networking;

using VRCMenuUtils;
using VRChat.UI;

using VRCModLoader;
using VRCTools;
using ModPrefs = VRCTools.ModPrefs;

using VRCExtended.Modules;
using VRCExtended.Config;
using VRCExtended.UI;
using VRCExtended.Storage;

using Newtonsoft.Json.Linq;

namespace VRCExtended
{
    /* NOTES:
     * Button.colors <- change UI color UwU
    */

    [VRCModInfo("VRCExtended", "1.0.0p1", "AtiLion", "https://github.com/AtiLion/VRCExtended/releases", "vrcextended")]
    internal class VRCExtended : VRCMod
    {
        #region Configuration Variables
        private List<MapConfig> _configWatch = new List<MapConfig>();
        private float _configLastCheck = 0f;
        #endregion

        #region VRCMod Functions
        void OnApplicationStart()
        {
            ExtendedLogger.Log("Loading VRCExtended...");

            // Load managers
            {
                // Load config
                ExtendedLogger.Log("Loading ManagerConfig...");
                ManagerConfig.Load();

                // Load storage
                ExtendedLogger.Log("Loading ManagerStorage...");
                ManagerStorage.Load();

                // Load modules
                ExtendedLogger.Log("Loading ManagerModule...");
                ManagerModule.Setup();
            }

            // Use VRCTools mod settings
            foreach (MapConfig map in ManagerConfig.Maps)
                LoadMapConfig(map);

            // Setup coroutines
            VRCMenuUtilsAPI.RunBeforeFlowManager(DisplayPreview());
#if !DEBUG
            VRCMenuUtilsAPI.RunBeforeFlowManager(CheckForUpdate());
#endif

            // Load UI
            ModManager.StartCoroutine(LoadUI());

            ExtendedLogger.Log("VRCExtended loaded!");
        }
        void OnFixedUpdate()
        {
            // Update configs
            _configLastCheck += Time.deltaTime;
            if (_configLastCheck > 2f)
            {
                bool save = false;
                foreach(MapConfig conf in _configWatch)
                {
                    bool reqBool = ModPrefs.GetBool(conf.Parent.GetType().Name, conf.Name);
                    if (reqBool != (bool)conf.Value)
                    {
                        ExtendedLogger.Log($"Found change in {conf.Name} of category {conf.Parent.GetType().Name} to {reqBool}");
                        conf.Value = reqBool;
                        save = true;
                    }
                }
                if (save)
                    ManagerConfig.Save();

                _configLastCheck = 0f;
            }
        }
        void OnApplicationQuit()
        {
            ExtendedLogger.Log("Saving ManagerStorage...");
            ManagerStorage.Save();
        }
        #endregion

        #region Configuration Functions
        void LoadMapConfig(MapConfig conf)
        {
            if(conf.MapType == EMapConfigType.CATEGORY)
            {
                ModPrefs.RegisterCategory(conf.Type.Name, conf.Name);
                foreach(MapConfig subConf in conf.Children)
                    LoadMapConfig(subConf);
            }
            else if(conf.Type == typeof(bool?))
            {
                ModPrefs.RegisterPrefBool(conf.Parent.GetType().Name, conf.Name, (bool)conf.Value, conf.Name, !conf.Visible);
                ModPrefs.SetBool(conf.Parent.GetType().Name, conf.Name, (bool)conf.Value);

                _configWatch.Add(conf);
            }
        }
        #endregion

        #region Coroutine Loaders
        private IEnumerator LoadUI()
        {
            // Wait for VRCMenuUtils
            yield return VRCMenuUtilsAPI.WaitForInit();

#if DEBUG
            // Load config UI
            ExtendedLogger.Log("Loading UIConfig...");
            yield return UIConfig.Setup();
#endif

            // Finish
            ExtendedLogger.Log("VRCExtended UI loaded!");
        }
        #endregion
        #region Coroutine Functions
        private IEnumerator DisplayPreview()
        {
            if (ManagerStorage.Storage.ContainsKey("previewShown"))
                yield break;
            bool popupOpen = true;

            VRCMenuUtilsAPI.Alert(
                "VRCE Preview",
                "This is a preview version of VRCExtended! Not all features are yet implemented, and expect bugs!",
                "Ok", () => {
                    VRCMenuUtilsAPI.HideCurrentPopup();
                    popupOpen = false;
                    ManagerStorage.Storage.Add("previewShown", true);
                });
            while (popupOpen) yield return null;
        }
        private IEnumerator CheckForUpdate()
        {
            bool popupOpen = false;

            // Check version information
            ExtendedLogger.Log("Checking for updates...");
            using(UnityWebRequest request = UnityWebRequest.Get("https://api.github.com/repos/AtiLion/VRCExtended/releases/latest"))
            {
                // Get data
                yield return request.SendWebRequest();

                // Check if data was downloaded successfully
                if(request.isNetworkError)
                {
                    ExtendedLogger.LogError($"Network error! Failed to check for updates! {request.error}");
                    yield break;
                }
                if (request.isHttpError)
                {
                    ExtendedLogger.LogError($"HTTP error! Failed to check for updates! {request.error}");
                    yield break;
                }

                // Check for update
                try
                {
                    JObject data = JObject.Parse(request.downloadHandler.text);
                    JToken version;

                    if(!data.TryGetValue("tag_name", out version))
                    {
                        ExtendedLogger.LogError("Could not find version data!");
                        yield break;
                    }

                    ExtendedLogger.Log($"Latest {(string)version} : Current {Version}");
                    if((string)version == Version)
                    {
                        ExtendedLogger.Log("No updates found!");
                        yield break;
                    }

                    popupOpen = true;
                }
                catch (Exception ex)
                {
                    ExtendedLogger.LogError("Version check failed!", ex);
                    yield break;
                }
            }

            // Display updates
            if(popupOpen)
            {
                ExtendedLogger.Log($"New update found!");
                VRCMenuUtilsAPI.Alert(
                        "VRCExtended Updater",
                        "A new version of VRCExtended is available! Press \"Open\" to open the download in your browser.",
                        "Close", () => { VRCMenuUtilsAPI.HideCurrentPopup(); popupOpen = false; },
                        "Open", () =>
                        {
                            VRCMenuUtilsAPI.HideCurrentPopup();
                            popupOpen = false;
                            System.Diagnostics.Process.Start("https://github.com/AtiLion/VRCExtended/releases");
                        }
                    );
                while (popupOpen) yield return null;
            }
        }
        #endregion
    }
}
