using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class ConfigUsers
    {
        [ConfigItem("Limit avatar features", false)]
        public bool? AvatarLimiter { get; set; }
        [ConfigItem("User specific volumes", false)]
        public bool? UserSpecificVolumes { get; set; }

        [ConfigItem("Get a light on your head[Local]", false)]
        public bool? HeadLight { get; set; }
        [ConfigItem("Sit on anyone's head", false)]
        public bool? SitOnAnyone { get; set; }
    }
}
