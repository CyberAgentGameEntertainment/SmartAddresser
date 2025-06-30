using System;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    /// <summary>
    ///     Provide the version based on the addressable entry's address.
    /// </summary>
    [Serializable]
    public sealed class AddressBasedVersionProvider : AddressBasedProvider, IVersionProvider
    {
        void IVersionProvider.Setup()
        {
            base.Setup();
        }

        string IVersionProvider.Provide(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            return base.Provide(address);
        }

        string IVersionProvider.GetDescription()
        {
            return base.GetDescription();
        }
    }
}
