using System;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    /// <summary>
    ///     Provide the label based on the addressable entry's address.
    /// </summary>
    [Serializable]
    public sealed class AddressBasedLabelProvider : AddressBasedProvider, ILabelProvider
    {
        void ILabelProvider.Setup()
        {
            base.Setup();
        }

        string ILabelProvider.Provide(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            return base.Provide(address);
        }

        string ILabelProvider.GetDescription()
        {
            return base.GetDescription();
        }
    }
}
