using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

using VRCExtended.Config;

namespace VRCExtended.Modules.General
{
    internal class FPSManager : IExtendedModule
    {
        #region FPSManager Properties
        public static int OriginalFrameRate { get; private set; } = 90;
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
            OriginalFrameRate = Application.targetFrameRate;

            // Set unlimited FPS
            if ((bool)ManagerConfig.Config.General.UnlimitedFPS)
                Application.targetFrameRate = 0;

            // Add unity scripts
            ManagerModule.ModuleObject.AddComponent<PauseDetector>();

            // Add unlimited FPS watcher
            ManagerConfig.WatchForUpdate("General.UnlimitedFPS", () =>
            {
                if ((bool)ManagerConfig.Config.General.UnlimitedFPS)
                    Application.targetFrameRate = 0;
                else
                    Application.targetFrameRate = OriginalFrameRate;
            });
        }
        #endregion

        #region Unity Scripts
        private class PauseDetector : MonoBehaviour
        {
            void OnApplicationFocus(bool hasFocus)
            {
                if (!(bool)ManagerConfig.Config.General.LowFPSUnfocused)
                    return;

                if(!hasFocus)
                {
                    Application.targetFrameRate = 5;
                }
                else
                {
                    if ((bool)ManagerConfig.Config.General.UnlimitedFPS)
                        Application.targetFrameRate = 0;
                    else
                        Application.targetFrameRate = OriginalFrameRate;
                }
            }
        }
        #endregion
    }
}
