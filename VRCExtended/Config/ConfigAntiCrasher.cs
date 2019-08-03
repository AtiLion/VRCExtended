using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCExtended.Config
{
    internal class ConfigAntiCrasher
    {
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
    }
}
