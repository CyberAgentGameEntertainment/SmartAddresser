using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Layouts
{
    [Serializable]
    public sealed class EntryError
    {
        [SerializeField] private EntryErrorType _type;
        [SerializeField] private string _message;

        public EntryError(EntryErrorType type, string message)
        {
            _type = type;
            _message = message;
        }

        public EntryErrorType Type => _type;
        public string Message => _message;
    }
}
