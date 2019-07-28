using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCExtended
{
    internal class AntiCrasherConfig
    {
        #region AntiCrasherConfig Properties
        public static AntiCrasherConfig Instance { get; private set; }
        #endregion

        #region Polygon Crasher
        public bool? PolyLimit { get; set; }
        public int? MaxPolygons { get; set; }
        #endregion

        #region Particle Crasher
        public bool? ParticleLimit { get; set; }
        //public bool DetectPotentialCrasher { get; set; }
        public int? MaxParticles { get; set; }
        #endregion

        #region Shader Crasher
        public bool? ShaderBlacklist { get; set; }
        public bool? UseOnlineBlacklist { get; set; }
        public bool? RemoveUnsupportedShaders { get; set; }
        public string[] BlacklistedShaders { get; set; }
        #endregion

        public AntiCrasherConfig() =>
            Instance = this;

        public bool CheckBackwardsCompatibility()
        {
            bool rebuilt = false;
            foreach(PropertyInfo property in typeof(AntiCrasherConfig).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                if(property.GetValue(this, null) == null)
                    rebuilt = true;

            if (PolyLimit == null)
                PolyLimit = true;
            if (MaxPolygons == null)
                MaxPolygons = 150000;

            if (ParticleLimit == null)
                ParticleLimit = true;
            if (MaxParticles == null)
                MaxParticles = 200;

            if (ShaderBlacklist == null)
                ShaderBlacklist = true;
            if (UseOnlineBlacklist == null)
                UseOnlineBlacklist = true;
            if (RemoveUnsupportedShaders == null)
                RemoveUnsupportedShaders = false;
            if(BlacklistedShaders == null)
            {
                BlacklistedShaders = new string[]
                {
                    "pretty",
                    "bluescreen",
                    "tesselation",
                    "tesselated",
                    "crasher",
                    "instant crash paralyzer",
                    "worldkill",
                    "tessellation",
                    "tessellated",
                    "oofer",
                    "starnest",
                    "xxx",
                    "dbtc",
                    "kyuzu",
                    "distancebased",
                    "waifuserp",
                    "loops",
                    "diebitch",
                    "thotdestroyer" // Thanks Herp Derpinstine
                };
            }

            if (rebuilt)
                ExtendedLogger.LogWarning("Anti-crasher configuration has been updated!");
            return rebuilt;
        }
        public static void CreateDefault()
        {
            AntiCrasherConfig acc = new AntiCrasherConfig();

            acc.CheckBackwardsCompatibility();
        }
    }
}
