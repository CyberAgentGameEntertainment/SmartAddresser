using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Foundation.AddressableAdapter
{
    internal sealed class AddressableAssetSettingsAdapter : IAddressableAssetSettingsAdapter
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
        public List<string> GetLabels()
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
