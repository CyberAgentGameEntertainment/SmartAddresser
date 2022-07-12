using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Core.Models.EntryRules.AddressRules
{
    /// <summary>
    ///     Provide addresses based on asset paths.
    /// </summary>
    [Serializable]
    public sealed class AssetPathBasedAddressProvider : IAddressProvider
    {
        public enum SourceType
        {
            FileName,
            FileNameWithoutExtensions,
            FilePath
        }

        [SerializeField] private SourceType _source = SourceType.FileName;
        [SerializeField] private bool _replaceWithRegex;
        [SerializeField] private string _pattern;
        [SerializeField] private string _replacement;

        private Regex _regex;

        /// <summary>
        ///     Source type of the address.
        /// </summary>
        public SourceType Source
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

        void IAddressProvider.Setup()
        {
            if (!_replaceWithRegex)
                return;
            _regex = new Regex(_pattern);
        }

        string IAddressProvider.CreateAddress(string assetPath, Type assetType, bool isFolder)
        {
            var sourceValue = CreateSourceValue(_source, assetPath);
            return _replaceWithRegex ? _regex.Replace(sourceValue, _replacement) : sourceValue;
        }

        private static string CreateSourceValue(SourceType self, string assetPath)
        {
            Assert.IsFalse(string.IsNullOrEmpty(assetPath));

            switch (self)
            {
                case SourceType.FileName:
                    return Path.GetFileName(assetPath);
                case SourceType.FileNameWithoutExtensions:
                    return Path.GetFileNameWithoutExtension(assetPath);
                case SourceType.FilePath:
                    return assetPath;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
