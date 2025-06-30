using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    [Serializable]
    public abstract class AddressableAssetGroupNameBasedProvider
    {
        [SerializeField] private bool _replaceWithRegex;
        [SerializeField] private string _pattern;
        [SerializeField] private string _replacement;

        private Regex _regex;

        /// <summary>
        ///     If true, replaces the group name value through regular expressions.
        /// </summary>
        public bool ReplaceWithRegex
        {
            get => _replaceWithRegex;
            set => _replaceWithRegex = value;
        }

        /// <summary>
        ///     Regex pattern to replace the group name value.
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

        public string Provide(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return null;

            if (_replaceWithRegex && _regex == null)
                return null;

            try
            {
                return _replaceWithRegex ? _regex.Replace(groupName, _replacement) : groupName;
            }
            catch
            {
                return null;
            }
        }

        public string GetDescription()
        {
            var result = "Source: Addressable Asset Group Name";
            if (_replaceWithRegex)
                result += $", Regex: Replace \"{_pattern}\" with \"{_replacement}\"";

            return result;
        }
    }
}
