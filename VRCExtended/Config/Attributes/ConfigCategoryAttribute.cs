using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCExtended.Config.Attributes
{
    internal class ConfigCategoryAttribute : Attribute
    {
        public string Name { get; private set; }
        public bool Visible { get; private set; }

        public ConfigCategoryAttribute(string name, bool visible = true)
        {
            this.Name = name;
            this.Visible = visible;
        }
    }
}
