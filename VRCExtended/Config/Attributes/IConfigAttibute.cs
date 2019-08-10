using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCExtended.Config.Attributes
{
    internal interface IConfigAttibute
    {
        string Name { get; }
        bool Visible { get; }
    }
}
