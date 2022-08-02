using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreAddressProviderAttribute : PropertyAttribute
    {
    }
}
