using System;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    /// <summary>
    ///     Provide the label based on the addressable asset group name.
    /// </summary>
    [Serializable]
    public sealed class AddressableAssetGroupNameBasedLabelProvider : AddressableAssetGroupNameBasedProvider,
        ILabelProvider
    {
        void ILabelProvider.Setup()
        {
            base.Setup();
        }

        string ILabelProvider.Provide(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            if (addressableAssetGroup == null)
                return null;

            return base.Provide(addressableAssetGroup.Name);
        }

        string ILabelProvider.GetDescription()
        {
            return base.GetDescription();
        }
    }
}
