using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class ConfigMain
    {
        [ConfigCategory("Anti Crasher")]
        public ConfigAntiCrasher AntiCrasher { get; set; }
    }
}
