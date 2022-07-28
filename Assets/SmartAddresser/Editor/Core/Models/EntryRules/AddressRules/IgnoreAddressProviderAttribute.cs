using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.EntryRules.AddressRules
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreAddressProviderAttribute : PropertyAttribute
    {
    }
}
