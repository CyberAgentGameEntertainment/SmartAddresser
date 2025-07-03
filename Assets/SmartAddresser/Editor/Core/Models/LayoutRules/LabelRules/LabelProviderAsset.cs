using System;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    public abstract class LabelProviderAsset : ScriptableObject, ILabelProvider
    {
        public abstract void Setup();

        public abstract string Provide(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup);

        public abstract string GetDescription();
    }
}
