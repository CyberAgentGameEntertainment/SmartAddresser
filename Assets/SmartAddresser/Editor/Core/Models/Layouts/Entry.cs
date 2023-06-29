using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Layouts
{
    [Serializable]
    public sealed class Entry
    {
        [SerializeField] private string _id;
        [SerializeField] private string _address;
        [SerializeField] private string _assetPath;
        [SerializeField] private string[] _labels;
        [SerializeField] private string[] _versions;
        [SerializeField] private LayoutErrorType _errorType;
        [SerializeField] private string _messages;
        [SerializeField] private List<EntryError> _errors = new List<EntryError>();

        [NonSerialized] private bool _isErrorTypeDirty = true;
        [NonSerialized] private bool _isMessagesDirty = true;

        public Entry(string address, string assetPath, IReadOnlyList<string> labels, IReadOnlyList<string> versions)
        {
            _id = IdentifierFactory.Create();
            _address = address;
            _assetPath = assetPath;
            // Sort labels and versions for sorting of LayoutViewer.
            _labels = labels == null ? Array.Empty<string>() : labels.OrderBy(x => x).ToArray();
            _versions = versions == null ? Array.Empty<string>() : versions.OrderBy(x => x).ToArray();
        }

        public string Id => _id;
        public string Address => _address;
        public string AssetPath => _assetPath;
        public string[] Labels => _labels;
        public string[] Versions => _versions;

        /// <summary>
        ///     Error type of the entry.
        /// </summary>
        public LayoutErrorType ErrorType
        {
            get
            {
                UpdateErrorType();
                return _errorType;
            }
        }

        /// <summary>
        ///     Warning and error messages.
        ///     This is the combined messages of <see cref="Errors" />.
        /// </summary>
        public string Messages
        {
            get
            {
                UpdateMessages();
                return _messages;
            }
        }

        /// <summary>
        ///     Warnings and errors of the entry.
        /// </summary>
        public List<EntryError> Errors => _errors;

        internal void UpdateErrorType()
        {
            if (!_isErrorTypeDirty)
                return;

            _errorType = LayoutErrorType.None;
            for (int i = 0, errorCount = _errors.Count; i < errorCount; i++)
            {
                var error = _errors[i];
                switch (error.Type)
                {
                    case EntryErrorType.Warning:
                        if (_errorType == LayoutErrorType.None)
                            _errorType = LayoutErrorType.Warning;
                        break;
                    case EntryErrorType.Error:
                        _errorType = LayoutErrorType.Error;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _isErrorTypeDirty = false;
        }

        internal void UpdateMessages()
        {
            if (!_isMessagesDirty)
                return;

            _messages = null;
            for (int i = 0, errorCount = _errors.Count; i < errorCount; i++)
            {
                var error = _errors[i];
                if (!string.IsNullOrEmpty(_messages))
                    _messages += Environment.NewLine;
                _messages += error.Message;
            }

            _isMessagesDirty = false;
        }

        internal void SetErrorTypeAndMessagesDirty()
        {
            _isErrorTypeDirty = true;
            _isMessagesDirty = true;
        }
    }
}
