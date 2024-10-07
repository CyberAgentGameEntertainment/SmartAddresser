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
        public IAddressableAssetEntryAdapter CreateOrMoveEntry(string groupName, string guid, bool invokeModificationEvent)
        {
            var group = _settings.FindGroup(groupName);
            var entry = _settings.CreateOrMoveEntry(guid, group, postEvent: invokeModificationEvent);
            return entry == null ? null : new AddressableAssetEntryAdapter(entry);
        }

        /// <inheritdoc />
        public bool RemoveEntry(string guid, bool invokeModificationEvent)
        {
            return _settings.RemoveAssetEntry(guid, invokeModificationEvent);
        }

        /// <inheritdoc />
        public void RemoveAllEntries(string groupName, bool invokeModificationEvent)
        {
            var group = _settings.groups.FirstOrDefault(x => x.Name == groupName);
            if (group == null)
                throw new InvalidOperationException($"Specified group '{groupName}' was not found.");

            var entries = group.entries.ToArray();
            foreach (var entry in entries)
                group.RemoveAssetEntry(entry, invokeModificationEvent);
        }

        public void InvokeBatchModificationEvent()
        {
            _settings.OnModification?.Invoke(_settings,
                AddressableAssetSettings.ModificationEvent.BatchModification,
                null);
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetLabels()
        {
            return _settings.GetLabels();
        }

        /// <inheritdoc />
        public void AddLabel(string label, bool invokeModificationEvent)
        {
            _settings.AddLabel(label, invokeModificationEvent);
        }

        /// <inheritdoc />
        public void RemoveLabel(string label, bool invokeModificationEvent)
        {
            _settings.RemoveLabel(label, invokeModificationEvent);
        }
    }
}
