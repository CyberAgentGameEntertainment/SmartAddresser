using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.TagRules
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreTagProviderAttribute : PropertyAttribute
    {
    }
}
