using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

using VRCExtended.Config;

namespace VRCExtended.Modules.General
{
    internal class LightingManager : IExtendedModule
    {
        #region LightingManager Properties
        public static Color OriginalAmbientLight { get; private set; }
        #endregion

        public void Setup()
        {
            // Setup event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        #region Unity Events
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex != 1)
                return;
            SceneManager.sceneLoaded -= OnSceneLoaded;

            // Save original values
            OriginalAmbientLight = RenderSettings.ambientLight;

            // Add unlimited FPS watcher
            ManagerConfig.WatchForUpdate("General.PartyMode", () =>
            {
                if ((bool)ManagerConfig.Config.General.PartyMode)
                    RenderSettings.ambientLight = Color.red;
                else
                    RenderSettings.ambientLight = OriginalAmbientLight;
            });
        }
        #endregion
    }
}
