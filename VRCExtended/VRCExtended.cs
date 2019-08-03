using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCModLoader;

namespace VRCExtended
{
    [VRCModInfo("VRCExtended", "1.0.0", "AtiLion", "https://github.com/AtiLion/VRCExtended/releases", "vrcextended")]
    internal class VRCExtended : VRCMod
    {
        #region VRCMod Functions
        void OnApplicationStart()
        {
            ExtendedLogger.Log("Loading VRCExtended...");
            ExtendedLogger.Log("VRCExtended loaded!");
        }
        #endregion
    }
}
