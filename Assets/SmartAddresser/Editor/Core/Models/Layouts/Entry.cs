using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Layouts
{
    [Serializable]
    public sealed class Entry
    {
        [SerializeField] private string _address;
        [SerializeField] private string _assetPath;
        [SerializeField] private string[] _labels;
        [SerializeField] private string[] _tags;
        [SerializeField] private LayoutErrorType _errorType;
        [SerializeField] private string _message;

        public Entry(string address, string assetPath, string[] labels, string[] tags, LayoutErrorType errorType,
            string message)
        {
            _address = address;
            _assetPath = assetPath;
            _labels = labels;
            _tags = tags;
            _errorType = errorType;
            _message = message;
        }

        public string Address => _address;
        public string AssetPath => _assetPath;
        public string[] Labels => _labels;
        public string[] Tags => _tags;
        public LayoutErrorType ErrorType => _errorType;
        public string Message => _message;
    }
}
