using System.Collections.Generic;
using SmartAddresser.Editor.Foundation.AddressableAdapter;

namespace SmartAddresser.Tests.Editor.Foundation
{
    internal sealed class FakeAddressableAssetEntryAdapter : IAddressableAssetEntryAdapter
    {
        public FakeAddressableAssetEntryAdapter(IEnumerable<string> labels = null)
        {
            Labels = labels == null ? new HashSet<string>() : new HashSet<string>(labels);
        }

        /// <inheritdoc />
        public string Address { get; private set; }

        /// <inheritdoc />
        public HashSet<string> Labels { get; }

        public string GroupName { get; private set; }

        /// <inheritdoc />
        public void SetAddress(string address, bool _)
        {
            Address = address;
        }

        /// <inheritdoc />
        public bool SetLabel(string label, bool enable, bool _)
        {
            if (enable)
                return Labels.Add(label);
            return Labels.Remove(label);
        }
        
        internal void SetGroup(string groupName)
        {
            GroupName = groupName;
        }
    }
}
