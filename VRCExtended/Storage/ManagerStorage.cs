using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VRCExtended.Storage
{
    internal static class ManagerStorage
    {
        #region StorageManager Information
        public static readonly string StorageFile = "VRCExtended.data";
        #endregion

        #region StorageManager Variables
        public static JObject Storage;
        #endregion

        #region StorageManager Events
        public static event Action OnSave;
        #endregion

        #region StorageManager Functions
        public static void Load()
        {
            if (Storage != null)
                return;
            bool forceSave = !File.Exists(StorageFile);


            // Check and parse config file
            if (!forceSave)
            {
                try { Storage = JObject.Parse(File.ReadAllText(StorageFile)); }
                catch (Exception ex) { ExtendedLogger.LogError("Failed to parse storage file. Maybe it got corrupted?", ex); }
            }

            // Check config and rebuild if null
            if (Storage == null)
                Storage = new JObject();

            // Save config if it doesn't exist
            if (forceSave)
                Save();
        }
        public static void Save()
        {
            ExtendedLogger.Log("Saving storage...");
            try { File.WriteAllText(StorageFile, Storage.ToString()); OnSave?.Invoke(); }
            catch (Exception ex) { ExtendedLogger.LogError("Failed to save storage file.", ex); }
            ExtendedLogger.Log("Storage saved!");
        }
        #endregion
    }
}
