using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VRCExtended
{
    internal static class VolumeControl
    {
        #region VolumeControl Variables
        private static JObject _volumes;
        #endregion

        #region VolumeControl Properties
        public static Dictionary<string, float[]> Volumes { get; private set; } = new Dictionary<string, float[]>();
        #endregion

        public static void Setup()
        {
            if (!File.Exists("volumes.json"))
                return;

            try
            {
                _volumes = JObject.Parse(File.ReadAllText("volumes.json"));

                foreach(JProperty property in _volumes.Properties())
                {
                    float[] volumes = new float[2] { 1f, 1f }; // 0 = User, 1 = Avatar
                    JArray jaVol = property.Value as JArray;

                    Volumes.Add(property.Name, volumes);
                    volumes[0] = (float)jaVol[0];
                    volumes[1] = (float)jaVol[1];
                }
                ExtendedLogger.Log("Successfully parsed volumes.json!");
            }
            catch(Exception ex)
            {
                ExtendedLogger.LogError("Failed to parse JSON volumes.json!", ex);
                return;
            }
        }

        #region VolumeControl Properties
        #endregion
    }
}
