using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Foundation.AddressableAdapter
{
    internal sealed class AddressableAssetEntryAdapter : IAddressableAssetEntryAdapter
    {
        private readonly AddressableAssetEntry _entry;

        public AddressableAssetEntryAdapter(AddressableAssetEntry entry)
        {
            _entry = entry;
        }

        /// <inheritdoc />
        public HashSet<string> Labels => _entry.labels;

        /// <inheritdoc />
        public bool SetLabel(string label, bool enable)
        {
            return _entry.SetLabel(label, enable);
        }
    }
}
