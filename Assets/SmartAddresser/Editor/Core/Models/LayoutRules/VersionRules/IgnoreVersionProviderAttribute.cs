using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreVersionProviderAttribute : PropertyAttribute
    {
    }
}
