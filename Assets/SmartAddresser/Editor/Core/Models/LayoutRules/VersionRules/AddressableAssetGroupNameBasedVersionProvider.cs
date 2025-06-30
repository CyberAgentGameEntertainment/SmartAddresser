using System;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    /// <summary>
    ///     Provide the version based on the addressable asset group name.
    /// </summary>
    [Serializable]
    public sealed class AddressableAssetGroupNameBasedVersionProvider : AddressableAssetGroupNameBasedProvider,
        IVersionProvider
    {
        void IVersionProvider.Setup()
        {
            base.Setup();
        }

        string IVersionProvider.Provide(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            if (addressableAssetGroup == null)
                return null;

            return base.Provide(addressableAssetGroup.Name);
        }

        string IVersionProvider.GetDescription()
        {
            return base.GetDescription();
        }
    }
}
