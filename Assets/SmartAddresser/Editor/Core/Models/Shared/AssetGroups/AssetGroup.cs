// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartAddresser.Editor.Foundation.OrderCollection;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
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
        [SerializeField] private StringObservableProperty _name = new StringObservableProperty("New Asset Group");
        [SerializeField] private AssetFilterObservableDictionary _filters = new AssetFilterObservableDictionary();
        [SerializeField] private StringOrderCollection _filterOrders = new StringOrderCollection();

        private readonly Subject<(string id, int index)> _filterOrderChangedSubject =
            new Subject<(string id, int index)>();

        private List<IAssetFilter> _cachedFilters = new List<IAssetFilter>();

        public AssetGroup()
        {
            _id = IdentifierFactory.Create();
        }

        public IObservable<(string id, int index)> FilterOrderChangedAsObservable => _filterOrderChangedSubject;

        public string Id => _id;

        public IObservableProperty<string> Name => _name;

        /// <summary>
        ///     Filter to determine whether an asset belongs to this group.
        /// </summary>
        public IReadOnlyObservableDictionary<string, IAssetFilter> Filters => _filters;

        public void Setup()
        {
            _cachedFilters.Clear();
            _cachedFilters.Capacity = _filters.Values.Count;
            foreach (var filter in _filters.Values)
            {
                filter?.SetupForMatching();

                // Cache filters for performance.
                _cachedFilters.Add(filter);
            }
        }

        /// <summary>
        ///     Return true if the asset belongs to this group.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        public bool Contains(string assetPath, Type assetType, bool isFolder)
        {
            if (_cachedFilters.Count == 0)
                return false;

            for (var i = 0; i < _cachedFilters.Count; i++)
            {
                var filter = _cachedFilters[i];
                if (filter == null)
                    continue;

                if (!filter.IsMatch(assetPath, assetType, isFolder))
                    return false;
            }

            return true;
        }

        public string GetDescription()
        {
            var result = new StringBuilder();
            var isFirstItem = true;
            foreach (var filter in _filters.Values.OrderBy(x => _filterOrders.GetIndex(x.Id)))
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

        public T AddFilter<T>() where T : IAssetFilter, new()
        {
            var filter = new T();
            AddFilter(filter);
            return filter;
        }

        public IAssetFilter AddFilter(Type type)
        {
            var filter = (IAssetFilter)Activator.CreateInstance(type);
            AddFilter(filter);
            return filter;
        }

        public void AddFilter<T>(T filter) where T : IAssetFilter
        {
            _filterOrders.Add(filter.Id);
            _filters.Add(filter.Id, filter);
        }

        public void RemoveFilter(string id)
        {
            _filters.Remove(id);
            _filterOrders.Remove(id);
        }

        public void ClearFilters()
        {
            var filterIds = _filters.Keys.ToArray();
            foreach (var filterId in filterIds)
                RemoveFilter(filterId);
        }

        public int GetFilterOrder(string id)
        {
            return _filterOrders.GetIndex(id);
        }

        public void SetFilterOrder(string id, int index)
        {
            _filterOrders.SetIndex(id, index);
            _filterOrderChangedSubject.OnNext((id, index));
        }

        public void OverwriteValuesFromJson(string from)
        {
            var fromObj = JsonUtility.FromJson<AssetGroup>(from);
            _name.Value = fromObj._name.Value;
            ClearFilters();

            foreach (var fromFilter in fromObj._filters.Values.OrderBy(x => fromObj.GetFilterOrder(x.Id)))
            {
                var fromFilterJson = JsonUtility.ToJson(fromFilter);
                var filter = AddFilter(fromFilter.GetType());
                filter.OverwriteValuesFromJson(fromFilterJson);
            }
        }
    }
}
