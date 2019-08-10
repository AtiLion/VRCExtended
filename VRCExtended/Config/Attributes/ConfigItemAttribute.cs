using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCExtended.Config.Attributes
{
    internal class ConfigItemAttribute : Attribute, IConfigAttibute
    {
        public string Name { get; private set; }
        public object DefaultValue { get; private set; }
        public bool Visible { get; private set; }

        public ConfigItemAttribute(string name, object defaultValue, bool visible = true)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;
            this.Visible = visible;
        }
    }
}
