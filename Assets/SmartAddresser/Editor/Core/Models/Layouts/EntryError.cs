using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Core.Models.Layouts
{
    [Serializable]
    public sealed class EntryError
    {
        [SerializeField] private EntryErrorType _type;
        [SerializeField] private string _message;

        private readonly Func<string> _getMessage;

        public EntryError(EntryErrorType type, Func<string> getMessage)
        {
            Assert.IsNotNull(getMessage);
            
            _type = type;
            _getMessage = getMessage;
        }

        public EntryErrorType Type => _type;

        public string Message
        {
            get
            {
                InvokeGetMessage();
                return _message;
            }
        }

        public void InvokeGetMessage()
        {
            if (!string.IsNullOrEmpty(_message))
                return;
            _message = _getMessage.Invoke();
        }
    }
}
