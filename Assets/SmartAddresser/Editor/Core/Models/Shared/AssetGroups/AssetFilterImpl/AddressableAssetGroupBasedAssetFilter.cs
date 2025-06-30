using System;
using System.Collections.Generic;
using System.Text;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    /// <summary>
    ///     Filter to pass assets if they belong to the specified AddressableAssetGroup.
    /// </summary>
    [Serializable]
    [AssetFilter("Addressable Group Filter", "Addressable Group Filter")]
    [RestrictedToRules(RuleType.Label, RuleType.Version)]
    public sealed class AddressableAssetGroupBasedAssetFilter : AssetFilterBase
    {
        [SerializeField]
        private ListableProperty<AddressableAssetGroup> _groups = new ListableProperty<AddressableAssetGroup>();

        private HashSet<AddressableAssetGroup> _groupSet = new HashSet<AddressableAssetGroup>();

        private bool _hasNullGroup;

        /// <summary>
        ///     AddressableAssetGroups for filtering.
        /// </summary>
        public ListableProperty<AddressableAssetGroup> Groups => _groups;

        public override void SetupForMatching()
        {
            _groupSet.Clear();
            _hasNullGroup = false;

            foreach (var group in _groups)
            {
                if (group == null)
                {
                    _hasNullGroup = true;
                    continue;
                }

                _groupSet.Add(group);
            }
        }

        public override bool Validate(out AssetFilterValidationError error)
        {
            if (_hasNullGroup)
            {
                error = new AssetFilterValidationError(this, new[] { "There are null reference groups." });
                return false;
            }

            error = null;
            return true;
        }

        /// <inheritdoc />
        public override bool IsMatch(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            if (addressableAssetGroup == null)
                return false;

            return _groupSet.Contains(addressableAssetGroup);
        }

        public override string GetDescription()
        {
            var result = new StringBuilder();
            var elementCount = 0;

            foreach (var group in _groups)
            {
                if (group == null) continue;

                if (elementCount >= 1) result.Append(" || ");

                result.Append(group.name);
                elementCount++;
            }

            if (result.Length >= 1)
            {
                if (elementCount >= 2)
                {
                    result.Insert(0, "( ");
                    result.Append(" )");
                }

                result.Insert(0, "Addressable Group: ");
            }

            return result.ToString();
        }
    }
}
