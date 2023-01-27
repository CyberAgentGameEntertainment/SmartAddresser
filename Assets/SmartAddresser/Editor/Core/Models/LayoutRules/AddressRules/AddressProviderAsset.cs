using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules
{
    public abstract class AddressProviderAsset : ScriptableObject, IAddressProvider
    {
        public abstract void Setup();
        public abstract string Provide(string assetPath, Type assetType, bool isFolder);
        public abstract string GetDescription();
    }
}
