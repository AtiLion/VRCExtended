using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class ConfigUsers
    {
#if DEBUG
        [ConfigItem("Limit avatar features", false)]
        public bool? AvatarLimiter { get; set; }
        [ConfigItem("User specific volumes", false)]
        public bool? UserSpecificVolumes { get; set; }
#endif

        [ConfigItem("Sit on anyone's head", false)]
        public bool? SitOnAnyone { get; set; }
    }
}
