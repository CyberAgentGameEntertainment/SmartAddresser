using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Foundation.AddressableAdapter
{
    public sealed class AddressableAssetEntryAdapter : IAddressableAssetEntryAdapter
    {
        private readonly AddressableAssetEntry _entry;

        public AddressableAssetEntryAdapter(AddressableAssetEntry entry)
        {
            _entry = entry;
        }
        
        /// <inheritdoc />
        public string Address => _entry.address;

        /// <inheritdoc />
        public HashSet<string> Labels => _entry.labels;
        
        /// <inheritdoc />
        public string GroupName => _entry.parentGroup.Name;

        /// <inheritdoc />
        public void SetAddress(string address, bool invokeModificationEvent)
        {
            _entry.SetAddress(address, invokeModificationEvent);
        }
        
        /// <inheritdoc />
        public bool SetLabel(string label, bool enable, bool invokeModificationEvent)
        {
            return _entry.SetLabel(label, enable, postEvent:invokeModificationEvent);
        }
    }
}
