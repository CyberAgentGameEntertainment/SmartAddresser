using System;
using System.Collections.Generic;
using System.Text;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups
{
    /// <summary>
    ///     Observable list of the <see cref="AssetGroup" />.
    /// </summary>
    [Serializable]
    public sealed class AssetGroupObservableList : ObservableList<AssetGroup>
    {
        /// <summary>
        ///     You need call this before use <see cref="Contains" />.
        /// </summary>
        public void Setup()
        {
            foreach (var group in this)
                group.Setup();
        }

        public bool Validate(out AssetGroupValidationError[] errors)
        {
            var groupErrors = new List<AssetGroupValidationError>();
            foreach (var group in this)
                if (!group.Validate(out var groupError))
                    groupErrors.Add(groupError);

            if (groupErrors.Count > 0)
            {
                errors = groupErrors.ToArray();
                return false;
            }

            errors = null;
            return true;
        }

        /// <summary>
        ///     Return true if this asset group contains the asset.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="address">The address assigned to the addressable entry. May be null when called from AddressRule.</param>
        /// <param name="addressableAssetGroup">The addressable asset group. May be null when called from AddressRule.</param>
        /// <returns></returns>
        public bool Contains(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            for (var i = 0; i < Count; i++)
                if (this[i].Contains(assetPath, assetType, isFolder, address, addressableAssetGroup))
                    return true;

            return false;
        }

        /// <summary>
        ///     Return description of the asset group.
        ///     If there is no asset groups, return null.
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            var groupDescriptions = new List<string>(Count);

            foreach (var group in this)
            {
                var groupDescription = group.GetDescription();

                if (string.IsNullOrEmpty(groupDescription))
                    continue;

                groupDescriptions.Add(groupDescription);
            }

            var groupDescriptionsCount = groupDescriptions.Count;
            if (groupDescriptionsCount == 0)
                return null;

            var description = new StringBuilder();
            for (var i = 0; i < groupDescriptionsCount; i++)
            {
                var groupDescription = groupDescriptions[i];

                if (i >= 1)
                    description.Append(" || ");

                if (groupDescriptionsCount >= 2)
                    description.Append(" (");

                description.Append(groupDescription);

                if (groupDescriptionsCount >= 2)
                    description.Append(") ");
            }

            return description.ToString();
        }
    }
}
