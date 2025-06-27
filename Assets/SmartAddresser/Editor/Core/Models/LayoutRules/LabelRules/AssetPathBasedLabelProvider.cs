using System;
using SmartAddresser.Editor.Core.Models.Shared;

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
            string addressableAssetGroupName)
        {
            return ((IProvider<string>)this).Provide(assetPath, assetType, isFolder);
        }

        public string GetDescription()
        {
            return ((IProvider<string>)this).GetDescription();
        }
    }
}
