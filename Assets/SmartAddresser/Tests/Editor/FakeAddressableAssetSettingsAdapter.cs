using System.Collections.Generic;
using SmartAddresser.Editor.Foundation.AddressableAdapter;

namespace SmartAddresser.Tests.Editor
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
                entry.GroupName = groupName;
            }
            else
            {
                // Create a new entry.
                var adapter = new FakeAddressableAssetEntryAdapter();
                entry = new Entry(guid, groupName, adapter);
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
        public List<string> GetLabels()
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
            public Entry(string assetGuid, string groupName, IAddressableAssetEntryAdapter adapter)
            {
                AssetGuid = assetGuid;
                GroupName = groupName;
                Adapter = adapter;
            }

            public string AssetGuid { get; }
            public string GroupName { get; set; }
            public IAddressableAssetEntryAdapter Adapter { get; }
        }
    }
}
