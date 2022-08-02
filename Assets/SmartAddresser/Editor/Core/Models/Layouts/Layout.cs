using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Layouts
{
    [Serializable]
    public sealed class Layout
    {
        [SerializeField] private string _id;
        [SerializeField] private LayoutErrorType _errorType;
        [SerializeField] private List<Group> _groups = new List<Group>();

        public Layout()
        {
            _id = IdentifierFactory.Create();
        }

        public LayoutErrorType ErrorType => _errorType;
        public List<Group> Groups => _groups;
        public bool HasValidated { get; private set; }

        /// <summary>
        ///     Validate all groups and entries in the layout.
        /// </summary>
        public void Validate()
        {
            var assetPathToEntries = new Dictionary<string, List<Entry>>();
            var addressToEntries = new Dictionary<string, List<Entry>>();
            var entryToGroup = new Dictionary<string, Group>();
            for (int i = 0, groupCount = _groups.Count; i < groupCount; i++)
            {
                var group = _groups[i];
                for (int j = 0, entryCount = group.Entries.Count; j < entryCount; j++)
                {
                    var entry = group.Entries[j];
                    entryToGroup.Add(entry.Id, group);

                    // Add AssetPath.
                    var assetPath = entry.AssetPath;
                    if (!assetPathToEntries.TryGetValue(assetPath, out var assetPathEntries))
                    {
                        assetPathEntries = new List<Entry>();
                        assetPathToEntries.Add(assetPath, assetPathEntries);
                    }

                    assetPathEntries.Add(entry);

                    // Add Address.
                    var address = entry.Address;
                    if (!addressToEntries.TryGetValue(address, out var addressEntries))
                    {
                        addressEntries = new List<Entry>();
                        addressToEntries.Add(address, addressEntries);
                    }

                    addressEntries.Add(entry);
                }
            }

            // If same assetPath is contained in multiple groups, mark as error.
            foreach (var assetPathEntries in assetPathToEntries)
            {
                var entries = assetPathEntries.Value;
                if (entries.Count <= 1)
                    break;

                // Create message.
                var message = "[Error] Duplicate Assets: This asset is included in following entries.";
                for (int i = 0, entryCount = entries.Count; i < entryCount; i++)
                {
                    var entry = entries[i];
                    var group = entryToGroup[entry.Id];
                    message +=
                        $"{Environment.NewLine}    - Group: {group.DisplayName}, Address: {entry.Address}, AssetPath: {entry.AssetPath}";
                }

                // Add error to each entry.
                for (int i = 0, entryCount = entries.Count; i < entryCount; i++)
                {
                    var entry = entries[i];
                    entry.Errors.Add(new EntryError(EntryErrorType.Error, message));
                }
            }

            // If same address is contained in multiple groups, mark as warning.
            foreach (var addressEntries in addressToEntries)
            {
                var entries = addressEntries.Value;

                if (entries.Count <= 1)
                    continue;

                // Create message.
                var message = "[Warning] Duplicate Addresses: This address is included in following entries.";
                for (int i = 0, entryCount = entries.Count; i < entryCount; i++)
                {
                    var entry = entries[i];
                    var group = entryToGroup[entry.Id];
                    message +=
                        $"{Environment.NewLine}    - Group: {group.DisplayName}, Address: {entry.Address}, AssetPath: {entry.AssetPath}";
                }

                // Add warning to each entry.
                for (int i = 0, entryCount = entries.Count; i < entryCount; i++)
                {
                    var entry = entries[i];
                    entry.Errors.Add(new EntryError(EntryErrorType.Warning, message));
                }
            }

            // Refresh error type of layout.
            RefreshErrorType();

            HasValidated = true;
        }

        private void RefreshErrorType()
        {
            _errorType = LayoutErrorType.None;
            for (int i = 0, groupCount = _groups.Count; i < groupCount; i++)
            {
                var group = _groups[i];
                group.RefreshErrorType();
                if (group.ErrorType.IsMoreCriticalThan(_errorType))
                    _errorType = group.ErrorType;
            }
        }
    }
}
