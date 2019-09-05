using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Harmony;

using VRCExtended.Modules.General;
using VRCExtended.Modules.UI;
using VRCExtended.Modules.LocalColliders;

using VRCModLoader;

namespace VRCExtended.Modules
{
    internal static class ManagerModule
    {
        #region ModuleObject Setup
        private static GameObject _moduleObject;
        public static GameObject ModuleObject
        {
            get
            {
                if(_moduleObject == null)
                {
                    _moduleObject = new GameObject("Extended Module Object");
                    GameObject.DontDestroyOnLoad(_moduleObject);
                }
                return _moduleObject;
            }
        }
        #endregion

        #region Harmony
        private static HarmonyInstance _harmony;
        public static HarmonyInstance Harmony
        {
            get
            {
                if (_harmony == null)
                    _harmony = HarmonyInstance.Create("vrcextended.modules");
                return _harmony;
            }
        }
        #endregion

        #region Modules
        private static IExtendedModule[] _toLoad = new IExtendedModule[]
        {
            new FPSManager(),
            new PortalManager(),
#if DEBUG
            new LightingManager(),
#endif
            new WorldsUI(),
            new ColliderController()
        };
        public static Dictionary<string, IExtendedModule> Modules { get; private set; } = new Dictionary<string, IExtendedModule>();
        #endregion

        public static void Setup()
        {
            // Load modules
            ExtendedLogger.Log("Loading modules...");
            foreach(IExtendedModule module in _toLoad)
            {
                try
                {
                    module.Setup();
                    ModManager.StartCoroutine(module.AsyncSetup());

                    Modules.Add(module.GetType().Name, module);
                    ExtendedLogger.Log($"Loaded module {module.GetType().Name}!");
                }
                catch (Exception ex)
                {
                    ExtendedLogger.LogError($"Error while trying to load module {module.GetType().Name}!", ex);
                }
            }
        }
    }
}
