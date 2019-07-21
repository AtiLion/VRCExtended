using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace VRCExtended
{
    public class LocalCollider : MonoBehaviour
    {
        void OnTriggerEnter(Collider collider)
        {
            ExtendedLogger.Log("Entered collider!");
        }
        void OnTriggerExit(Collider collider)
        {
            ExtendedLogger.Log("Exited collider!");
        }
    }
}
