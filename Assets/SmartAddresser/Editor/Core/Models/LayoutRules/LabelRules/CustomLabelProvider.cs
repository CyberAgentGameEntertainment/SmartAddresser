using System;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    [Serializable]
    public sealed class CustomLabelProvider : ILabelProvider
    {
        public LabelProviderAsset labelProvider;

        public void Setup()
        {
            if (labelProvider == null)
                return;

            labelProvider.Setup();
        }

        public string Provide(string assetPath, Type assetType, bool isFolder, string address, AddressableAssetGroup addressableAssetGroup)
        {
            if (labelProvider == null)
                return null;

            return labelProvider.Provide(assetPath, assetType, isFolder, address, addressableAssetGroup);
        }

        public string GetDescription()
        {
            if (labelProvider == null)
                return string.Empty;

            return labelProvider.GetDescription();
        }
    }
}
