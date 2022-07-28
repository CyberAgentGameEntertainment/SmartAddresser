using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.EntryRules.LabelRules
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreLabelProviderAttribute : PropertyAttribute
    {
    }
}
