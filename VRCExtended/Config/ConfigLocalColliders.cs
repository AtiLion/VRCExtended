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
        public bool? Enabled { get; set; }
        [ConfigItem("Enable colliders on hands only", true)]
        public bool? EnableForHandsOnly { get; set; }
        [ConfigItem("Enable colliders on legs", false)]
        public bool? EnableForLegs { get; set; }
        [ConfigItem("Disable colliders on distance", true)]
        public bool? DisableOnDistance { get; set; }
        [ConfigItem("Require manual enable", false)]
        public bool? ManualEnable { get; set; }
#if DEBUG
        [ConfigItem("Use smart colliders", false)]
        public bool? UseSmartColliders { get; set; }
#endif

        [ConfigItem("Players can interact with other players", false)]
        public bool? PlayersInteractWithOthers { get; set; }
        [ConfigItem("Players can interact with you", true)]
        public bool? PlayersInteractWithMe { get; set; }
        
        [ConfigItem("Add fake colliders to yourself", false)]
        public bool? FakeCollidersMe { get; set; }
        [ConfigItem("Add fake colliders to others", false)]
        public bool? FakeCollidersOthers { get; set; }

#if DEBUG
        [ConfigItem("Grabbable colliders", false)]
        public bool? GrabbableColliders { get; set; }
#endif
    }
}
