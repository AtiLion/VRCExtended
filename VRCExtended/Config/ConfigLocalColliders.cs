using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class ConfigLocalColliders
    {
        [ConfigItem("Enabled", false)]
        public bool Enabled { get; set; }

        public bool AllPlayerColliders { get; set; }
    }
}
