using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class ConfigAntiCrasher
    {
        [ConfigItem("Enabled", false)]
        public bool? Enabled { get; set; }

        #region Polygon Crasher
        [ConfigItem("Polygon limit", true)]
        public bool? PolyLimit { get; set; }
        [ConfigItem("Maximum polygons", 150000)]
        public int? MaxPolygons { get; set; }
        #endregion

        #region Particle Crasher
        [ConfigItem("Limit particles", true)]
        public bool? ParticleLimit { get; set; }
#if DEBUG
        [ConfigItem("Detect potential crashers", false)]
        public bool? DetectPotentialCrasher { get; set; }
#endif
        [ConfigItem("Max particles per avatar", 600)]
        public int? MaxParticles { get; set; }
        #endregion

        #region Shader Crasher
        [ConfigItem("Remove blacklisted shaders", true)]
        public bool? ShaderBlacklist { get; set; }
        [ConfigItem("Download latest shader blacklist", true)]
        public bool? UseOnlineBlacklist { get; set; }
        [ConfigItem("Remove unsupported shaders", false)]
        public bool? RemoveUnsupportedShaders { get; set; }
        [ConfigItem("Block all shaders", false)]
        public bool? BlockAllShaders { get; set; }
        [ConfigItem("Shader blacklist", new string[0], false)]
        public string[] BlacklistedShaders { get; set; }
        #endregion
    }
}
