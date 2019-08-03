using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class ConfigImprovements
    {
        [ConfigItem("Ask to use portal", true)]
        public bool? askUserPortal { get; set; }
        [ConfigItem("Disable portals", false)]
        public bool? disablePortal { get; set; }
#if DEBUG
        [ConfigItem("Limit avatar features", false)]
        public bool? avatarLimiter { get; set; }
#endif
    }
}
