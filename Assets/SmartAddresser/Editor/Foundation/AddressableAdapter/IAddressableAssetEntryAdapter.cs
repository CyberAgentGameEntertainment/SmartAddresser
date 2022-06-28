using System.Collections.Generic;

namespace SmartAddresser.Editor.Foundation.AddressableAdapter
{
    /// <summary>
    ///     Abstraction of operations to AddressableAssetEntry.
    /// </summary>
    internal interface IAddressableAssetEntryAdapter
    {
        /// <summary>
        ///     The set of labels for this entry.
        /// </summary>
        HashSet<string> Labels { get; }

        /// <summary>
        ///     Set or unset a label on this entry.
        /// </summary>
        /// <param name="label">The label name.</param>
        /// <param name="enable">Setting to true will add the label, false will remove it.</param>
        /// <returns></returns>
        bool SetLabel(string label, bool enable);
    }
}
