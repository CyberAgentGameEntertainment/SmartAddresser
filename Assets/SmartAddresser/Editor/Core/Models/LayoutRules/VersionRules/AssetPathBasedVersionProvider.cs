using System;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    /// <summary>
    ///     Provide the tag based on asset paths.
    /// </summary>
    [Serializable]
    public sealed class AssetPathBasedVersionProvider : AssetPathBasedProvider, IVersionProvider
    {
        void IVersionProvider.Setup()
        {
            base.Setup();
        }

        string IVersionProvider.Provide(string assetPath, Type assetType, bool isFolder, string address, AddressableAssetGroup addressableAssetGroup)
        {
            // Asset path based provider doesn't use address or addressableAssetGroupName
            return base.Provide(assetPath, assetType, isFolder);
        }

        string IVersionProvider.GetDescription()
        {
            return base.GetDescription();
        }
    }
}
