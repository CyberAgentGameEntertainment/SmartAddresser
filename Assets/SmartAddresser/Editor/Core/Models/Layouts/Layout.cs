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

        [NonSerialized] private bool _isErrorTypeDirty = true;

        public Layout()
        {
            _id = IdentifierFactory.Create();
        }

        public LayoutErrorType ErrorType
        {
            get
            {
                UpdateErrorType();
                return _errorType;
            }
        }

        public List<Group> Groups => _groups;
        public bool HasValidated { get; private set; }

        /// <summary>
        ///     Validate all groups and entries in the layout.
        /// </summary>
        /// <param name="updateErrorTypeAndMessages">Update all error types and messages after the validation.</param>
        public void Validate(bool updateErrorTypeAndMessages = false)
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

                    // If the entry has multiple versions, mark as error.
                    if (entry.Versions.Length >= 2)
                    {
                        string CreateMessage()
                        {
                            return "[Error] Multiple Versions: This asset has multiple versions.";
                        }

                        entry.Errors.Add(new EntryError(EntryErrorType.Error, CreateMessage));
                    }
                }
            }

            // If same assetPath is contained in multiple groups, mark as error.
            foreach (var assetPathEntries in assetPathToEntries)
            {
                var entries = assetPathEntries.Value;
                if (entries.Count <= 1)
                    continue;

                // Create message.
                string CreateMessage()
                {
                    var message = "[Error] Duplicate Assets: This asset is included in following entries.";
                    for (int i = 0, entryCount = entries.Count; i < entryCount; i++)
                    {
                        var entry = entries[i];
                        var group = entryToGroup[entry.Id];
                        message +=
                            $"{Environment.NewLine}    - Group: {group.DisplayName}, Address: {entry.Address}, AssetPath: {entry.AssetPath}";
                    }

                    return message;
                }

                // Add error to each entry.
                for (int i = 0, entryCount = entries.Count; i < entryCount; i++)
                {
                    var entry = entries[i];
                    entry.Errors.Add(new EntryError(EntryErrorType.Error, CreateMessage));
                }
            }

            // If same address is contained in multiple groups, mark as warning.
            foreach (var addressEntries in addressToEntries)
            {
                var entries = addressEntries.Value;

                if (entries.Count <= 1)
                    continue;

                // Create message.
                string CreateMessage()
                {
                    var message = "[Warning] Duplicate Addresses: This address is included in following entries.";
                    for (int i = 0, entryCount = entries.Count; i < entryCount; i++)
                    {
                        var entry = entries[i];
                        var group = entryToGroup[entry.Id];
                        message +=
                            $"{Environment.NewLine}    - Group: {group.DisplayName}, Address: {entry.Address}, AssetPath: {entry.AssetPath}";
                    }

                    return message;
                }

                // Add warning to each entry.
                for (int i = 0, entryCount = entries.Count; i < entryCount; i++)
                {
                    var entry = entries[i];
                    entry.Errors.Add(new EntryError(EntryErrorType.Warning, CreateMessage));
                }
            }

            // Set the dirty flag of the error type.
            SetErrorTypeDirty();

            if (updateErrorTypeAndMessages)
            {
                UpdateErrorTypeAndMessages();
            }

            HasValidated = true;
        }

        private void SetErrorTypeDirty()
        {
            _isErrorTypeDirty = true;
            for (int i = 0, groupCount = _groups.Count; i < groupCount; i++)
            {
                var group = _groups[i];
                group.SetErrorTypeDirty();
            }
        }

        private void UpdateErrorType()
        {
            if (!_isErrorTypeDirty) 
                return;
            
            _errorType = LayoutErrorType.None;
            for (int i = 0, groupCount = _groups.Count; i < groupCount; i++)
            {
                var group = _groups[i];
                var groupErrorType = group.ErrorType;
                if (groupErrorType.IsMoreCriticalThan(_errorType))
                    _errorType = groupErrorType;
            }

            _isErrorTypeDirty = false;
        }

        private void UpdateErrorTypeAndMessages()
        {
            if (!_isErrorTypeDirty) 
                return;
            
            _errorType = LayoutErrorType.None;
            for (int i = 0, groupCount = _groups.Count; i < groupCount; i++)
            {
                var group = _groups[i];
                var groupErrorType = group.ErrorType;
                if (groupErrorType.IsMoreCriticalThan(_errorType))
                    _errorType = groupErrorType;
                
                group.UpdateMessages();
            }

            _isErrorTypeDirty = false;
        }
    }
}
