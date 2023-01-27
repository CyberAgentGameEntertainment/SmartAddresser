using System;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    [Serializable]
    public sealed class CustomVersionProvider : IVersionProvider
    {
        public VersionProviderAsset versionProvider;

        public void Setup()
        {
            versionProvider.Setup();
        }

        public string Provide(string assetPath, Type assetType, bool isFolder)
        {
            return versionProvider.Provide(assetPath, assetType, isFolder);
        }

        public string GetDescription()
        {
            if (versionProvider == null)
                return string.Empty;

            return versionProvider.GetDescription();
        }
    }
}
