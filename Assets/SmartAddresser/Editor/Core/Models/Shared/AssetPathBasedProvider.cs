using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    [Serializable]
    public abstract class AssetPathBasedProvider
    {
        [SerializeField] private PartialAssetPathType _source = PartialAssetPathType.AssetPath;
        [SerializeField] private bool _replaceWithRegex;
        [SerializeField] private string _pattern;
        [SerializeField] private string _replacement;

        private Regex _regex;

        /// <summary>
        ///     Source type of the address.
        /// </summary>
        public PartialAssetPathType Source
        {
            get => _source;
            set => _source = value;
        }

        /// <summary>
        ///     If true, replaces the source value through regular expressions.
        /// </summary>
        public bool ReplaceWithRegex
        {
            get => _replaceWithRegex;
            set => _replaceWithRegex = value;
        }

        /// <summary>
        ///     Regex pattern to replace the source value.
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

        public string Provide(string assetPath, Type assetType, bool isFolder)
        {
            if (_replaceWithRegex && _regex == null)
                return null;

            try
            {
                var sourceValue = _source.Create(assetPath);
                return _replaceWithRegex ? _regex.Replace(sourceValue, _replacement) : sourceValue;
            }
            catch
            {
                return null;
            }
        }

        public string GetDescription()
        {
            var result = $"Source: {_source.ToString()}";
            if (_replaceWithRegex)
                result += $", Regex: Replace \"{_pattern}\" with \"{_replacement}\"";

            return result;
        }
    }
}
