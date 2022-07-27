using System;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Layouts
{
    [Serializable]
    public sealed class Group
    {
        [SerializeField] private AddressableAssetGroup _addressableGroup;
        [SerializeField] private LayoutErrorType _errorType;
        [SerializeField] private List<Entry> _entries = new List<Entry>();

        public Group(AddressableAssetGroup addressableGroup)
        {
            _addressableGroup = addressableGroup;
        }

        public AddressableAssetGroup AddressableGroup => _addressableGroup;
        public LayoutErrorType ErrorType => _errorType;
        public List<Entry> Entries => _entries;

        public void RefreshErrorType()
        {
            foreach (var entry in _entries)
                switch (entry.ErrorType)
                {
                    case LayoutErrorType.None:
                        break;
                    case LayoutErrorType.Warning:
                        if (_errorType == LayoutErrorType.None)
                            _errorType = LayoutErrorType.Warning;
                        break;
                    case LayoutErrorType.Error:
                        _errorType = LayoutErrorType.Error;
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }
    }
}
