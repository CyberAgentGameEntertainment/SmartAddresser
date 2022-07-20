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
    [Serializable]
    public sealed class AssetGroupCollection
    {
        [SerializeField] private ObservableAssetGroupDictionary _groups = new ObservableAssetGroupDictionary();
        [SerializeField] private StringOrderCollection _orders = new StringOrderCollection();

        private readonly ObservableProperty<string> _description = new ObservableProperty<string>();
        private readonly Subject<(string id, int index)> _orderChangedSubject = new Subject<(string id, int index)>();
        private readonly List<AssetGroup> _orderedGroups = new List<AssetGroup>();

        public IObservable<(string id, int index)> OrderChangedAsObservable => _orderChangedSubject;
        public IReadOnlyObservableDictionary<string, AssetGroup> Groups => _groups;
        public IReadOnlyObservableProperty<string> Description => _description;

        public AssetGroup Add()
        {
            var group = new AssetGroup();
            Add(group);
            return group;
        }

        internal void Add(AssetGroup group)
        {
            _orders.Add(group.Id);
            _groups.Add(group.Id, group);
        }

        public void Remove(string id)
        {
            _groups.Remove(id);
            _orders.Remove(id);
        }

        public int GetOrder(string id)
        {
            return _orders.GetIndex(id);
        }

        public void SetOrder(string id, int index)
        {
            _orders.SetIndex(id, index);
            _orderChangedSubject.OnNext((id, index));
        }

        internal void RefreshDescription()
        {
            var groupDescriptions = new List<string>();

            foreach (var group in _groups.Values.OrderBy(x => _orders.GetIndex(x.Id)))
            {
                var groupDescription = group.GetDescription();

                if (string.IsNullOrEmpty(groupDescription))
                    continue;

                groupDescriptions.Add(groupDescription);
            }

            if (groupDescriptions.Count == 0)
            {
                _description.Value = "(None)";
                return;
            }

            var description = new StringBuilder();
            for (var i = 0; i < groupDescriptions.Count; i++)
            {
                var groupDescription = groupDescriptions[i];

                if (i >= 1)
                    description.Append(" || ");

                if (groupDescriptions.Count >= 2)
                    description.Append(" (");

                description.Append(groupDescription);

                if (groupDescriptions.Count >= 2)
                    description.Append(") ");
            }

            _description.Value = description.ToString();
        }

        internal void Setup()
        {
            _orderedGroups.Clear();
            foreach (var group in _groups.Values.OrderBy(x => _orders.GetIndex(x.Id)))
            {
                group.Setup();
                // Cache ordered AssetGroups for performance.
                _orderedGroups.Add(group);
            }
        }

        internal bool IsTargetAsset(string assetPath, Type assetType, bool isFolder)
        {
            for (var i = 0; i < _orderedGroups.Count; i++)
                if (_orderedGroups[i].Contains(assetPath, assetType, isFolder))
                    return true;

            return false;
        }
    }
}
