using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    [Serializable]
    public abstract class AddressBasedProvider
    {
        [SerializeField] private bool _replaceWithRegex;
        [SerializeField] private string _pattern;
        [SerializeField] private string _replacement;

        private Regex _regex;

        /// <summary>
        ///     If true, replaces the address value through regular expressions.
        /// </summary>
        public bool ReplaceWithRegex
        {
            get => _replaceWithRegex;
            set => _replaceWithRegex = value;
        }

        /// <summary>
        ///     Regex pattern to replace the address value.
        /// </summary>
        public string Pattern
        {
            get => _pattern;
            set => _pattern = value;
        }

        /// <summary>
        ///     Replacement value for the regex pattern.
        /// </summary>
        public string Replacement
        {
            get => _replacement;
            set => _replacement = value;
        }

        public void Setup()
        {
            if (!_replaceWithRegex)
                return;

            try
            {
                _regex = new Regex(_pattern);
            }
            catch
            {
                _regex = null;
            }
        }

        public string Provide(string address)
        {
            if (string.IsNullOrEmpty(address))
                return null;

            if (_replaceWithRegex && _regex == null)
                return null;

            try
            {
                var sourceValue = address;
                return _replaceWithRegex ? _regex.Replace(sourceValue, _replacement) : sourceValue;
            }
            catch
            {
                return null;
            }
        }

        public string GetDescription()
        {
            var result = "Source: Address";
            if (_replaceWithRegex)
                result += $", Regex: Replace \"{_pattern}\" with \"{_replacement}\"";

            return result;
        }
    }
}
