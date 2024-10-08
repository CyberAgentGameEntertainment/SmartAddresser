using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Foundation.AddressableAdapter
{
    /// <summary>
    ///     Abstraction of operations to AddressableAssetSettings.
    /// </summary>
    public interface IAddressableAssetSettingsAdapter
    {
        /// <summary>
        ///     Find and asset entry by guid.
        /// </summary>
        /// <param name="guid">The asset guid.</param>
        /// <returns>The found entry adapter or null.</returns>
        IAddressableAssetEntryAdapter FindAssetEntry(string guid);

        /// <summary>
        ///     Create a new entry, or if one exists in a different group, move it into the new group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="guid">The asset guid.</param>
        /// <param name="invokeModificationEvent">
        ///     If true, call <see cref="AddressableAssetSettings.OnModification" /> after
        ///     creating or moving.
        /// </param>
        /// <returns>The created entry adapter.</returns>
        IAddressableAssetEntryAdapter CreateOrMoveEntry(string groupName, string guid, bool invokeModificationEvent);

        /// <summary>
        ///     Remove an asset entry.
        /// </summary>
        /// <param name="guid">The  guid of the asset.</param>
        /// <param name="invokeModificationEvent">
        ///     If true, call <see cref="AddressableAssetSettings.OnModification" /> after
        ///     removing.
        /// </param>
        /// <returns>True if the entry was found and removed.</returns>
        bool RemoveEntry(string guid, bool invokeModificationEvent);

        /// <summary>
        ///     Remove all asset entries.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="invokeModificationEvent">
        ///     If true, call <see cref="AddressableAssetSettings.OnModification" /> after
        ///     removing each entry.
        /// </param>
        void RemoveAllEntries(string groupName, bool invokeModificationEvent);

        /// <summary>
        ///     Invoke the modification event.
        /// </summary>
        void InvokeBatchModificationEvent();

        /// <summary>
        ///     Gets the list of all defined labels.
        /// </summary>
        /// <returns>Returns a list of all defined labels.</returns>
        IReadOnlyList<string> GetLabels();

        /// <summary>
        ///     Add a new label.
        /// </summary>
        /// <param name="label">The label name.</param>
        /// <param name="invokeModificationEvent">
        ///     If true, call <see cref="AddressableAssetSettings.OnModification" /> after
        ///     creating or moving.
        /// </param>
        void AddLabel(string label, bool invokeModificationEvent);

        /// <summary>
        ///     Remove a label by name.
        /// </summary>
        /// <param name="label">The label name.</param>
        /// <param name="invokeModificationEvent">
        ///     If true, call <see cref="AddressableAssetSettings.OnModification" /> after
        ///     creating or moving.
        /// </param>
        void RemoveLabel(string label, bool invokeModificationEvent);
    }
}
