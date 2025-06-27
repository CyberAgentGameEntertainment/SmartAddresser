using System;
using SmartAddresser.Editor.Core.Models.Shared;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    /// <summary>
    ///     Provide the tag based on asset paths.
    /// </summary>
    [Serializable]
    public sealed class AssetPathBasedVersionProvider : AssetPathBasedProvider, IVersionProvider
    {
        public void Setup()
        {
            ((IProvider<string>)this).Setup();
        }

        public string Provide(string assetPath, Type assetType, bool isFolder, string address, string addressableAssetGroupName)
        {
            // Asset path based provider doesn't use address or addressableAssetGroupName
            return ((IProvider<string>)this).Provide(assetPath, assetType, isFolder);
        }

        public string GetDescription()
        {
            return ((IProvider<string>)this).GetDescription();
        }
    }
}
