using System;

namespace SmartAddresser.Editor.Foundation.CustomDrawers
{
    public class CustomGUIDrawer : Attribute
    {
        public CustomGUIDrawer(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; }
    }
}
