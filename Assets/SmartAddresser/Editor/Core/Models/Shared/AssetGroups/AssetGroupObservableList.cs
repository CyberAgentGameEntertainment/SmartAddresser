using System;
using System.Collections.Generic;
using System.Text;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;

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

        /// <summary>
        ///     Return true if this asset group contains the asset.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        public bool Contains(string assetPath, Type assetType, bool isFolder)
        {
            for (var i = 0; i < Count; i++)
                if (this[i].Contains(assetPath, assetType, isFolder))
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
