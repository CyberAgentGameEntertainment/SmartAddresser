using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreAssetFilterAttribute : PropertyAttribute
    {
    }
}
