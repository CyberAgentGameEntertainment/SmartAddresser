using System;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    [Serializable]
    public sealed class CustomVersionProvider : IVersionProvider
    {
        public VersionProviderAsset versionProvider;

        public void Setup()
        {
            if (versionProvider == null)
                return;
            
            versionProvider.Setup();
        }

        public string Provide(string assetPath, Type assetType, bool isFolder, string address, string addressableAssetGroupName)
        {
            if (versionProvider == null)
                return null;
            
            return versionProvider.Provide(assetPath, assetType, isFolder, address, addressableAssetGroupName);
        }

        public string GetDescription()
        {
            if (versionProvider == null)
                return string.Empty;

            return versionProvider.GetDescription();
        }
    }
}
