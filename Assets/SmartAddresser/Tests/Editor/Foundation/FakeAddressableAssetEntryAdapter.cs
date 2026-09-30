using System.Collections.Generic;
using SmartAddresser.Editor.Foundation.AddressableAdapter;

namespace SmartAddresser.Tests.Editor.Foundation
{
    internal sealed class FakeAddressableAssetEntryAdapter : IAddressableAssetEntryAdapter
    {
        public FakeAddressableAssetEntryAdapter(IEnumerable<string> labels = null)
        {
            _labels = labels == null ? new SortedSet<string>() : new SortedSet<string>(labels);
        }

        /// <inheritdoc />
        public string Address { get; private set; }

        /// <inheritdoc />
        public ISet<string> Labels => _labels;
        private SortedSet<string> _labels;

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
                return _labels.Add(label);
            return _labels.Remove(label);
        }
        
        internal void SetGroup(string groupName)
        {
            GroupName = groupName;
        }
    }
}
