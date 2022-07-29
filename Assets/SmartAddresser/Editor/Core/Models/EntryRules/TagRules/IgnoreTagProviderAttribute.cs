using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.EntryRules.TagRules
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreTagProviderAttribute : PropertyAttribute
    {
    }
}
