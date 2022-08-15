using System.Collections.Generic;

namespace SmartAddresser.Editor.Foundation.AddressableAdapter
{
    /// <summary>
    ///     Abstraction of operations to AddressableAssetEntry.
    /// </summary>
    public interface IAddressableAssetEntryAdapter
    {
        /// <summary>
        ///     Address of the entry.
        /// </summary>
        string Address { get; }

        /// <summary>
        ///     The set of labels for this entry.
        /// </summary>
        HashSet<string> Labels { get; }

        /// <summary>
        ///     Name of the parent addressable group.
        /// </summary>
        string GroupName { get; }

        /// <summary>
        ///     Set a address of the entry.
        /// </summary>
        /// <param name="address"></param>
        void SetAddress(string address);

        /// <summary>
        ///     Set or unset a label on this entry.
        /// </summary>
        /// <param name="label">The label name.</param>
        /// <param name="enable">Setting to true will add the label, false will remove it.</param>
        /// <returns></returns>
        bool SetLabel(string label, bool enable);
    }
}
