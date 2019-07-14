using System;
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
        public bool PolyLimit { get; set; }
        public int MaxPolygons { get; set; }
        #endregion

        #region Particle Crasher
        public bool ParticleLimit { get; set; }
        //public bool DetectPotentialCrasher { get; set; }
        public int MaxParticles { get; set; }
        #endregion

        #region Shader Crasher
        public bool ShaderBlacklist { get; set; }
        //public bool UseOnlineBlacklist { get; set; }
        public string[] BlacklistedShaders { get; set; }
        #endregion

        public AntiCrasherConfig() =>
            Instance = this;

        public static void CreateDefault()
        {
            AntiCrasherConfig acc = new AntiCrasherConfig();

            acc.PolyLimit = true;
            acc.MaxPolygons = 150000;

            acc.ParticleLimit = true;
            //acc.DetectPotentialCrasher = false;
            acc.MaxParticles = 200;

            acc.ShaderBlacklist = true;
            //acc.UseOnlineBlacklist = false;
            acc.BlacklistedShaders = new string[]
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
                "izzy",
                "star",
                "diebitch"
            };
        }
    }
}
