// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups
{
    /// <summary>
    ///     Rules for defining a group of assets.
    /// </summary>
    [Serializable]
    public sealed class AssetGroup
    {
        [SerializeField] private string _id;
        [SerializeField] private ObservableProperty<string> _name = new ObservableProperty<string>("New Asset Group");

        [SerializeField] private SerializeReferenceObservableList<IAssetFilter> _filters =
            new SerializeReferenceObservableList<IAssetFilter>();

        public AssetGroup()
        {
            _id = IdentifierFactory.Create();
        }

        public string Id => _id;

        public IObservableProperty<string> Name => _name;

        /// <summary>
        ///     Filter to determine whether an asset belongs to this group.
        /// </summary>
        public IObservableList<IAssetFilter> Filters => _filters;

        public void Setup()
        {
            foreach (var filter in _filters)
                filter?.SetupForMatching();
        }

        public bool Validate(out AssetGroupValidationError error)
        {
            var filterErrors = new List<AssetFilterValidationError>();
            foreach (var filter in _filters)
                if (!filter.Validate(out var filterError))
                    filterErrors.Add(filterError);

            if (filterErrors.Count > 0)
            {
                error = new AssetGroupValidationError(this, filterErrors.ToArray());
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        ///     Return true if the asset belongs to this group.
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
            if (_filters.Count == 0)
                return false;

            for (var i = 0; i < _filters.Count; i++)
            {
                var filter = _filters[i];
                if (filter == null)
                    continue;

                if (!filter.IsMatch(assetPath, assetType, isFolder, address, addressableAssetGroup))
                    return false;
            }

            return true;
        }

        public string GetDescription()
        {
            var result = new StringBuilder();
            var isFirstItem = true;
            foreach (var filter in _filters)
            {
                var description = filter.GetDescription();

                if (string.IsNullOrEmpty(description))
                    continue;

                if (!isFirstItem)
                    result.Append(" && ");

                result.Append(description);
                isFirstItem = false;
            }

            return result.ToString();
        }

        public void OverwriteValuesFromJson(string from)
        {
            var fromObj = JsonUtility.FromJson<AssetGroup>(from);
            _name.Value = fromObj._name.Value;
            _filters.Clear();

            foreach (var fromFilter in fromObj._filters)
            {
                var fromFilterJson = JsonUtility.ToJson(fromFilter);
                var filter = (IAssetFilter)Activator.CreateInstance(fromFilter.GetType());
                _filters.Add(filter);
                filter.OverwriteValuesFromJson(fromFilterJson);
            }
        }
    }
}
