using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCExtended.Config
{
    internal class ConfigImprovements
    {
        public bool askUserPortal { get; set; }
        public bool disablePortal { get; set; }
        public bool antiCrasher { get; set; }
#if DEBUG
        public bool avatarLimiter { get; set; }
#endif
    }
}
