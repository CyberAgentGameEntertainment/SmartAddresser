using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Foundation.AddressableAdapter
{
    public sealed class AddressableAssetSettingsAdapter : IAddressableAssetSettingsAdapter
    {
        private readonly AddressableAssetSettings _settings;

        public AddressableAssetSettingsAdapter(AddressableAssetSettings settings)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        public IAddressableAssetEntryAdapter FindAssetEntry(string guid)
        {
            var entry = _settings.FindAssetEntry(guid);
            return entry == null ? null : new AddressableAssetEntryAdapter(entry);
        }

        /// <inheritdoc />
        public IAddressableAssetEntryAdapter CreateOrMoveEntry(string groupName, string guid)
        {
            var group = _settings.FindGroup(groupName);
            var entry = _settings.CreateOrMoveEntry(guid, group);
            return entry == null ? null : new AddressableAssetEntryAdapter(entry);
        }

        /// <inheritdoc />
        public bool RemoveEntry(string guid)
        {
            return _settings.RemoveAssetEntry(guid);
        }

        /// <inheritdoc />
        public void RemoveAllEntries(string groupName)
        {
            var group = _settings.groups.FirstOrDefault(x => x.Name == groupName);
            if (group == null)
                throw new InvalidOperationException($"Specified group '{groupName}' was not found.");

            var entries = group.entries.ToArray();
            foreach (var entry in entries)
                group.RemoveAssetEntry(entry);
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetLabels()
        {
            return _settings.GetLabels();
        }

        /// <inheritdoc />
        public void AddLabel(string label)
        {
            _settings.AddLabel(label);
        }

        /// <inheritdoc />
        public void RemoveLabel(string label)
        {
            _settings.RemoveLabel(label);
        }
    }
}
