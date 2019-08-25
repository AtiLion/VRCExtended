using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC.Core;
using VRC.UI;

using VRCMenuUtils;

using VRCModLoader;
using VRCTools;
using ModPrefs = VRCTools.ModPrefs;

using VRCExtended.Modules;
using VRCExtended.Config;
using VRCExtended.UI;

namespace VRCExtended
{
    /* NOTES:
     * ApiGroup.FetchGroupNames(ownerId, ApiGroup.World.value, success, fail) - Get worlds of user
     * ApiGroup.FetchGroupNames(ownerId, ApiGroup.Avatar.value, success, fail) - Get avatars of user[unconfirmed]
     * Button.colors <- change UI color UwU
    */

    [VRCModInfo("VRCExtended", "1.0.0", "AtiLion", "https://github.com/AtiLion/VRCExtended/releases", "vrcextended")]
    internal class VRCExtended : VRCMod
    {
        #region Configuration Variables
        private List<MapConfig> _configWatch = new List<MapConfig>();
        private DateTime _configLastCheck;
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

                // Load modules
                ExtendedLogger.Log("Loading ManagerModule...");
                ManagerModule.Setup();
            }

            // Use VRCTools mod settings
            foreach (MapConfig map in ManagerConfig.Maps)
                LoadMapConfig(map);

            // Load UI
            ModManager.StartCoroutine(LoadUI());

            ExtendedLogger.Log("VRCExtended loaded!");
        }
        void OnFixedUpdate()
        {
            // Update configs
            if(_configLastCheck == null || (DateTime.Now - _configLastCheck).TotalMilliseconds >= 1000) // Update every second
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

                _configLastCheck = DateTime.Now;
            }
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
        private static IEnumerator LoadUI()
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
    }
}
