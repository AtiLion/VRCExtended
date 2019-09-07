using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace VRCExtended.Config
{
    internal static class ManagerConfig
    {
        #region ConfigManager Information
        public static readonly string ConfigFile = "VRCExtended.json";
        #endregion

        #region ConfigManager Variables
        private static ConfigMain _config;

        private static List<MapConfig> _maps;
        #endregion

        #region ConfigManager Properties
        public static ConfigMain Config
        {
            get
            {
                if (_config == null)
                    Load();
                return _config;
            }
        }
        public static List<MapConfig> Maps
        {
            get
            {
                if (_maps == null)
                    Load();
                return _maps;
            }
        }
        #endregion

        #region ConfigManager Events
        public delegate void ConfigValueUpdateHandler(MapConfig conf);
        public static event ConfigValueUpdateHandler OnValueUpdate;
        public static event Action OnSave;
        #endregion

        #region ConfigManager Functions
        public static void Load()
        {
            if (_config != null)
                return;
            bool forceSave = !File.Exists(ConfigFile);
            

            // Check and parse config file
            if (!forceSave)
            {
                try { _config = JsonConvert.DeserializeObject<ConfigMain>(File.ReadAllText(ConfigFile)); }
                catch (Exception ex) { ExtendedLogger.LogError("Failed to parse configuration file. Maybe it got corrupted?", ex); }
            }

            // Check config and rebuild if null
            if (_config == null)
                _config = new ConfigMain();

            // Map and add defaults
            _maps = new List<MapConfig>();
            foreach (PropertyInfo property in typeof(ConfigMain).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                MapConfig map = new MapConfig(property, _config, ref forceSave, OnConfigValueUpdate);

                if (map.Property != null)
                    _maps.Add(map);
            }

            // Save config if it doesn't exist
            if (forceSave)
                Save();
        }
        public static void Save()
        {
            ExtendedLogger.Log("Saving configuration...");
            try { File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(Config, Formatting.Indented)); OnSave?.Invoke(); }
            catch (Exception ex) { ExtendedLogger.LogError("Failed to save configuration file.", ex); }
            ExtendedLogger.Log("Configuration saved!");
        }

        public static bool WatchForUpdate(string path, Action func)
        {
            string[] fullPath = path.Split('.');
            MapConfig map = null;
            int index = 0;

            map = _maps.FirstOrDefault(a => a.Property.Name == fullPath[0]);
            while(map != null && map.MapType != EMapConfigType.ITEM && index < fullPath.Length)
            {
                index++;
                map = map.Children.FirstOrDefault(a => a.Property.Name == fullPath[index]);
            }

            if(map == null)
            {
                ExtendedLogger.LogError($"Failed to setup watcher for {path}! Map not found!");
                return false;
            }
            else if(map.MapType != EMapConfigType.ITEM)
            {
                ExtendedLogger.LogError($"Failed to setup watcher for {path}! Provided map is not an item!");
                return false;
            }

            OnValueUpdate += (MapConfig conf) =>
            {
                if (conf != map)
                    return;

                try
                {
                    func();
                }
                catch (Exception ex)
                {
                    ExtendedLogger.LogError($"Error while executing watcher for {path}!", ex);
                }
            };
            return true;
        }
        #endregion

        #region ConfigManager Event Handlers
        private static void OnConfigValueUpdate(MapConfig map) =>
            OnValueUpdate?.Invoke(map);
        #endregion
    }
}
