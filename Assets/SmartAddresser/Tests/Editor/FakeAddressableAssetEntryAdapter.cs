using System.Collections.Generic;
using SmartAddresser.Editor.Foundation.AddressableAdapter;

namespace SmartAddresser.Tests.Editor
{
    internal sealed class FakeAddressableAssetEntryAdapter : IAddressableAssetEntryAdapter
    {
        public FakeAddressableAssetEntryAdapter(IEnumerable<string> labels = null)
        {
            Labels = labels == null ? new HashSet<string>() : new HashSet<string>(labels);
        }

        /// <inheritdoc />
        public HashSet<string> Labels { get; }

        /// <inheritdoc />
        public bool SetLabel(string label, bool enable)
        {
            if (enable)
                return Labels.Add(label);
            return Labels.Remove(label);
        }
    }
}
