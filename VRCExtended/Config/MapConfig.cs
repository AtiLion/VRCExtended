using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class MapConfig
    {
        #region Map Properties
        public string Name { get; private set; }
        public bool Visible { get; private set; }
        public PropertyInfo Property { get; private set; }
        public EMapConfigType MapType { get; private set; }
        public object Parent { get; private set; }
        #endregion

        #region Config Properties
        public Type Type => Property?.PropertyType;
        public object Value
        {
            get => Property?.GetValue(Parent, null);
            set => Property?.SetValue(Parent, value, null);
        }
        public List<MapConfig> Children { get; private set; }
        #endregion

        public MapConfig(PropertyInfo property, object parent, ref bool forceSave)
        {
            // Load attribute
            IConfigAttibute configAttibute = (IConfigAttibute)property.GetCustomAttributes(typeof(IConfigAttibute), true).FirstOrDefault();
            if (configAttibute == null)
                return;

            // Setup map properties
            Property = property;
            Parent = parent;
            Name = configAttibute.Name;
            Visible = configAttibute.Visible;
            if (configAttibute.GetType() == typeof(ConfigCategoryAttribute))
                MapType = EMapConfigType.CATEGORY;
            else
                MapType = EMapConfigType.ITEM;

            // Setup config properties
            if(MapType == EMapConfigType.CATEGORY)
            {
                if (Value == null)
                    Value = Activator.CreateInstance(Type);
                Children = new List<MapConfig>();

                foreach(PropertyInfo childProperty in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    MapConfig map = new MapConfig(childProperty, Value, ref forceSave);

                    if (map.Property != null)
                        Children.Add(map);
                }
            }
            else
            {
                if (Value == null)
                {
                    Value = ((ConfigItemAttribute)configAttibute).DefaultValue;
                    forceSave = true;
                }
            } 
        }
    }

    internal enum EMapConfigType
    {
        CATEGORY,
        ITEM
    }
}
