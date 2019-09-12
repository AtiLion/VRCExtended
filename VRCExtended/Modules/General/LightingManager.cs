using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using VRCExtended.Config;
using VRCExtended.VRChat;

namespace VRCExtended.Modules.General
{
#if DEBUG
    internal class LightingManager : IExtendedModule
    {
        #region LightingManager Properties
        public static Color OriginalAmbientLight { get; private set; }
        #endregion

        public void Setup() {}
        public IEnumerator AsyncSetup()
        {
            // Wait for VRChat to load
            yield return VRCEManager.WaitForVRChatLoad();

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
    }
#endif
}
