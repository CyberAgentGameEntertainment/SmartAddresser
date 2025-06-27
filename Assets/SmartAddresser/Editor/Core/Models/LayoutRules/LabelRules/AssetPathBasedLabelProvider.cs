using System;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    /// <summary>
    ///     Provide the label based on asset paths.
    /// </summary>
    [Serializable]
    public sealed class AssetPathBasedLabelProvider : AssetPathBasedProvider, ILabelProvider
    {
        public void Setup()
        {
            ((IProvider<string>)this).Setup();
        }

        public string Provide(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            return ((IProvider<string>)this).Provide(assetPath, assetType, isFolder);
        }

        public string GetDescription()
        {
            return ((IProvider<string>)this).GetDescription();
        }
    }
}
