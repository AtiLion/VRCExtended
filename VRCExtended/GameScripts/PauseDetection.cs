using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using VRCTools;

namespace VRCExtended.GameScripts
{
    public class PauseDetection : MonoBehaviour
    {
        void OnApplicationFocus(bool hasFocus)
        {
            if (!ModPrefs.GetBool("vrcextended", "fpsManagement"))
                return;

            if (!hasFocus)
            {
                Application.targetFrameRate = 5;
                ExtendedLogger.Log("Game out of focus, setting FPS to " + Application.targetFrameRate);
            }
            else
            {
                if (ModPrefs.GetBool("vrcextended", "unlimitedFPS"))
                    Application.targetFrameRate = 0;
                else
                    Application.targetFrameRate = VRCExtended.FrameRate;
                ExtendedLogger.Log("Game in focus, setting FPS to " + Application.targetFrameRate);
            }
        }
    }
}
