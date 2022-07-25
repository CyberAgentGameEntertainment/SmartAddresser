using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Layouts
{
    [Serializable]
    public sealed class Layout
    {
        [SerializeField] private LayoutErrorType _errorType;
        [SerializeField] private List<Group> _groups = new List<Group>();

        public LayoutErrorType ErrorType => _errorType;
        public List<Group> Groups => _groups;

        public void RefreshErrorType()
        {
            foreach (var group in _groups)
            {
                group.RefreshErrorType();
                switch (group.ErrorType)
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
}
