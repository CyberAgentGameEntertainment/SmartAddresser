using System.Collections.Generic;
using SmartAddresser.Editor.Foundation.AddressableAdapter;

namespace SmartAddresser.Tests.Editor.Foundation
{
    internal sealed class FakeAddressableAssetSettingsAdapter : IAddressableAssetSettingsAdapter
    {
        private readonly Dictionary<string, Entry> _guidToEntryMap = new Dictionary<string, Entry>();

        private readonly List<string> _labels = new List<string>();

        public IAddressableAssetEntryAdapter FindAssetEntry(string guid)
        {
            return _guidToEntryMap.TryGetValue(guid, out var entry) ? entry.Adapter : null;
        }

        /// <inheritdoc />
        public IAddressableAssetEntryAdapter CreateOrMoveEntry(string groupName, string guid)
        {
            if (_guidToEntryMap.TryGetValue(guid, out var entry))
            {
                // Change group of the existing entry.
                entry.Adapter.SetGroup(groupName);
            }
            else
            {
                // Create a new entry.
                var adapter = new FakeAddressableAssetEntryAdapter();
                adapter.SetGroup(groupName);
                entry = new Entry(guid, adapter);
                _guidToEntryMap.Add(guid, entry);
            }

            return entry.Adapter;
        }

        /// <inheritdoc />
        public bool RemoveEntry(string guid)
        {
            return _guidToEntryMap.Remove(guid);
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetLabels()
        {
            return _labels;
        }

        /// <inheritdoc />
        public void AddLabel(string label)
        {
            _labels.Add(label);
        }

        /// <inheritdoc />
        public void RemoveLabel(string label)
        {
            _labels.Remove(label);
        }

        private class Entry
        {
            public Entry(string assetGuid, FakeAddressableAssetEntryAdapter adapter)
            {
                AssetGuid = assetGuid;
                Adapter = adapter;
            }

            public string AssetGuid { get; }
            public FakeAddressableAssetEntryAdapter Adapter { get; }
        }
    }
}
