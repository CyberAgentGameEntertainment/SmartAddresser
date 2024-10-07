using System.Collections.Generic;
using System.Linq;
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
        public IAddressableAssetEntryAdapter CreateOrMoveEntry(string groupName, string guid, bool invokeModificationEvent)
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
        public bool RemoveEntry(string guid, bool invokeModificationEvent)
        {
            return _guidToEntryMap.Remove(guid);
        }

        public void RemoveAllEntries(string groupName, bool invokeModificationEvent)
        {
            var targetGuids = _guidToEntryMap
                .Values
                .Where(x => x.Adapter.GroupName == groupName)
                .Select(x => x.AssetGuid)
                .ToArray();
            foreach (var guid in targetGuids)
                _guidToEntryMap.Remove(guid);
        }

        public void InvokeBatchModificationEvent()
        {
            // Do nothing when testing.
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetLabels()
        {
            return _labels;
        }

        /// <inheritdoc />
        public void AddLabel(string label, bool _)
        {
            _labels.Add(label);
        }

        /// <inheritdoc />
        public void RemoveLabel(string label, bool _)
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
