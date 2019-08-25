using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class ConfigGeneral
    {
        [ConfigItem("Lower FPS when game is unfocused", false)]
        public bool? LowFPSUnfocused { get; set; }
        [ConfigItem("Remove FPS limit", false)]
        public bool? UnlimitedFPS { get; set; }

        [ConfigItem("Ask to use portal", true)]
        public bool? AskUsePortal { get; set; }
        [ConfigItem("Disable portals", false)]
        public bool? DisablePortals { get; set; }

#if DEBUG
        // Requires the new config menu to be set
        [ConfigItem("World brightness", 0f)]
        public float? Brightness { get; set; }
        [ConfigItem("Party mode[Seizure warning]", false)]
        public bool? PartyMode { get; set; }
#endif
    }
}
