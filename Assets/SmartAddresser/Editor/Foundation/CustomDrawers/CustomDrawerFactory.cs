using System;
using System.Reflection;
using UnityEditor;

namespace SmartAddresser.Editor.Foundation.CustomDrawers
{
    public static class CustomDrawerFactory
    {
        public static ICustomDrawer Create(Type type)
        {
            var customDrawerType = GetCustomDrawerType(type);

            if (customDrawerType == null)
                return null;

            var instance = Activator.CreateInstance(customDrawerType);
            return instance as ICustomDrawer;
        }

        private static Type GetCustomDrawerType(Type type)
        {
            var targetTypes = TypeCache.GetTypesWithAttribute<CustomGUIDrawer>();

            foreach (var targetType in targetTypes)
            {
                var attr = targetType.GetCustomAttribute<CustomGUIDrawer>();
                if (attr.TargetType == type)
                    return targetType;
            }

            return null;
        }
    }
}
