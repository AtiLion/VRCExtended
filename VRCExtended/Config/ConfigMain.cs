using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class ConfigMain
    {
        [ConfigCategory("General")]
        public ConfigGeneral General { get; set; }

        [ConfigCategory("UI")]
        public ConfigUI UI { get; set; }

        [ConfigCategory("Local Colliders")]
        public ConfigLocalColliders LocalColliders { get; set; }

        [ConfigCategory("Anti Crasher")]
        public ConfigAntiCrasher AntiCrasher { get; set; }

        [ConfigCategory("Users")]
        public ConfigUsers Users { get; set; }
    }
}
