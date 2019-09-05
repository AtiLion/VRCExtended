using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine.SceneManagement;

namespace VRCExtended.VRChat
{
    internal static class VRCEManager
    {
        #region Load Event Variables
        private static bool _initalLoad = false;
        private static bool _loadEventHooked = false;
        #endregion
        #region Load Event Coroutines
        public static IEnumerator WaitForVRChatLoad()
        {
            // Already loaded
            if (_initalLoad)
                yield break;

            if(!_loadEventHooked)
            {
                // Add scene events
                SceneManager.sceneLoaded += loadEventSceneExecutor;

                _loadEventHooked = true;
            }

            // Wait for load
            while (!_initalLoad)
                yield return null;
        }
        #endregion
        #region Load Event Executors
        private static void loadEventSceneExecutor(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex != 1)
                return;

            _initalLoad = true;
            SceneManager.sceneLoaded -= loadEventSceneExecutor;
        }
        #endregion
    }
}
