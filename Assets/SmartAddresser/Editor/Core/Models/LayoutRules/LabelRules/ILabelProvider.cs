using System;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    /// <summary>
    ///     Provides a label for addressable asset entries with awareness of address and group information.
    /// </summary>
    public interface ILabelProvider
    {
        /// <summary>
        ///     Setup the label provider.
        /// </summary>
        void Setup();

        /// <summary>
        ///     Provide a label for the addressable asset.
        /// </summary>
        /// <param name="assetPath">The asset path.</param>
        /// <param name="assetType">The type of the asset.</param>
        /// <param name="isFolder">The asset is folder or not.</param>
        /// <param name="address">The address assigned to the addressable entry.</param>
        /// <param name="addressableAssetGroup">The addressable asset group.</param>
        /// <returns>Returns the label. If the label cannot be provided, returns null.</returns>
        string Provide(string assetPath, Type assetType, bool isFolder, string address, AddressableAssetGroup addressableAssetGroup);

        /// <summary>
        ///     Get the description of this rule for display in the UI.
        /// </summary>
        /// <returns></returns>
        string GetDescription();
    }
}
