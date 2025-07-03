using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    /// <summary>
    ///     Filter to pass assets if the address matches the regex.
    /// </summary>
    [Serializable]
    [AssetFilter("Address Filter", "Address Filter")]
    [RestrictedToRules(RuleType.Label, RuleType.Version)]
    public sealed class AddressBasedAssetFilter : AssetFilterBase
    {
        [SerializeField] private bool _matchWithFolders;
        [SerializeField] private AssetFilterCondition _condition = AssetFilterCondition.ContainsMatched;
        [SerializeField] private StringListableProperty _addressRegex = new StringListableProperty();
        private List<string> _errorRegexStrings = new List<string>();
        private List<Regex> _regexes = new List<Regex>();

        public bool MatchWithFolders
        {
            get => _matchWithFolders;
            set => _matchWithFolders = value;
        }

        public AssetFilterCondition Condition
        {
            get => _condition;
            set => _condition = value;
        }

        /// <summary>
        ///     Regex string for address matching.
        /// </summary>
        public StringListableProperty AddressRegex => _addressRegex;

        public override void SetupForMatching()
        {
            _regexes.Clear();
            _errorRegexStrings.Clear();
            foreach (var addressRegex in _addressRegex)
            {
                if (string.IsNullOrEmpty(addressRegex))
                    continue;

                try
                {
                    var regex = new Regex(addressRegex);
                    _regexes.Add(regex);
                }
                catch
                {
                    // If the regex string is invalid and an exception is thrown, add to errorStrings and continue.
                    _errorRegexStrings.Add(addressRegex);
                }
            }
        }

        public override bool Validate(out AssetFilterValidationError error)
        {
            if (_errorRegexStrings.Count >= 1)
            {
                error = new AssetFilterValidationError(
                    this,
                    _errorRegexStrings
                        .Select(errorRegexString => $"Invalid regex string: {errorRegexString}")
                        .ToArray());
                return false;
            }

            error = null;
            return true;
        }

        /// <inheritdoc />
        public override bool IsMatch(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            if (string.IsNullOrEmpty(address))
                return false;

            if (!_matchWithFolders && isFolder)
                return false;

            switch (_condition)
            {
                case AssetFilterCondition.ContainsMatched:
                    for (int i = 0, size = _regexes.Count; i < size; i++)
                        if (_regexes[i].IsMatch(address))
                            return true;
                    return false;
                case AssetFilterCondition.MatchAll:
                    for (int i = 0, size = _regexes.Count; i < size; i++)
                        if (!_regexes[i].IsMatch(address))
                            return false;
                    return true;
                case AssetFilterCondition.ContainsUnmatched:
                    for (int i = 0, size = _regexes.Count; i < size; i++)
                        if (!_regexes[i].IsMatch(address))
                            return true;
                    return false;
                case AssetFilterCondition.NotMatchAll:
                    for (int i = 0, size = _regexes.Count; i < size; i++)
                        if (_regexes[i].IsMatch(address))
                            return false;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string GetDescription()
        {
            var result = new StringBuilder();
            var elementCount = 0;
            foreach (var addressRegex in _addressRegex)
            {
                if (string.IsNullOrEmpty(addressRegex))
                    continue;

                if (elementCount >= 1)
                {
                    var delimiter =
                        _condition == AssetFilterCondition.MatchAll || _condition == AssetFilterCondition.NotMatchAll
                            ? " && "
                            : " || ";
                    result.Append(delimiter);
                }

                result.Append(addressRegex);
                elementCount++;
            }

            if (result.Length >= 1)
            {
                if (elementCount >= 2)
                {
                    result.Insert(0, "( ");
                    result.Append(" )");
                }

                var prefix = _condition == AssetFilterCondition.MatchAll ||
                             _condition == AssetFilterCondition.ContainsMatched
                    ? "Address Match: "
                    : "Address Not Match: ";
                result.Insert(0, prefix);
            }

            return result.ToString();
        }
    }
}
