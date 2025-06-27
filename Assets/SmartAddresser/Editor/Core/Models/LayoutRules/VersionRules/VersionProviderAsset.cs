using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    public abstract class VersionProviderAsset : ScriptableObject, IVersionProvider
    {
        public abstract void Setup();
        public abstract string Provide(string assetPath, Type assetType, bool isFolder, string address, string addressableAssetGroupName);
        public abstract string GetDescription();
    }
}
