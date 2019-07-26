using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRChat;

using UnityEngine;

namespace VRCExtended.GameScripts
{
    public class LocalCollider : MonoBehaviour
    {
        void OnTriggerEnter(Collider collider)
        {
            if (collider.tag != "handCollider")
                return;
            /*if (VRCEPlayer.Instance.Avatar == collider.gameObject)
                return;*/

            ExtendedLogger.Log("Entered collider!");
        }
        void OnTriggerExit(Collider collider)
        {
            if (collider.tag != "handCollider")
                return;
            /*if (VRCEPlayer.Instance.Avatar == collider.gameObject)
                return;*/

            ExtendedLogger.Log("Exited collider!");
        }
    }
}
