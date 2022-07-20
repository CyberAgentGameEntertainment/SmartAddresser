using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AssetFilterAttribute : PropertyAttribute
    {
        public AssetFilterAttribute(string menuName, string displayName)
        {
            MenuName = menuName;
            DisplayName = displayName;
        }

        public string MenuName { get; }
        public string DisplayName { get; }
    }
}
