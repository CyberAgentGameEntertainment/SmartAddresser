using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    /// <summary>
    ///     Filter to pass assets if matches the type.
    /// </summary>
    [Serializable]
    [AssetFilter("Type Filter", "Type Filter")]
    public sealed class TypeBasedAssetFilter : AssetFilterBase
    {
        [SerializeField] private TypeReferenceListableProperty _type = new TypeReferenceListableProperty();
        [SerializeField] private bool _matchWithDerivedTypes = true;
        [SerializeField] private bool _invertMatch;
        private List<string> _invalidAssemblyQualifiedNames = new List<string>();

        private Dictionary<Type, bool> _resultCache = new Dictionary<Type, bool>();
        private object _resultCacheLocker = new object();
        private List<Type> _types = new List<Type>();

        /// <summary>
        ///     Type.
        /// </summary>
        public TypeReferenceListableProperty Type => _type;

        public bool MatchWithDerivedTypes
        {
            get => _matchWithDerivedTypes;
            set => _matchWithDerivedTypes = value;
        }

        /// <summary>
        ///     If true, the result of the match will be inverted.
        /// </summary>
        public bool InvertMatch
        {
            get => _invertMatch;
            set => _invertMatch = value;
        }

        public override void SetupForMatching()
        {
            _types.Clear();
            _invalidAssemblyQualifiedNames.Clear();
            foreach (var typeRef in _type)
            {
                if (typeRef == null)
                    continue;

                if (!typeRef.IsValid())
                    continue;

                var assemblyQualifiedName = typeRef.AssemblyQualifiedName;
                var type = System.Type.GetType(assemblyQualifiedName);
                if (type == null)
                    _invalidAssemblyQualifiedNames.Add(assemblyQualifiedName);
                else
                    _types.Add(type);
            }

            _resultCache.Clear();
        }

        public override bool Validate(out AssetFilterValidationError error)
        {
            if (_invalidAssemblyQualifiedNames.Count >= 1)
            {
                error = new AssetFilterValidationError(
                    this,
                    _invalidAssemblyQualifiedNames
                        .Select(qualifiedName => $"Invalid type reference: {qualifiedName}")
                        .ToArray());
                return false;
            }

            error = null;
            return true;
        }

        /// <inheritdoc />
        public override bool IsMatch(string assetPath, Type assetType, bool isFolder, string address, AddressableAssetGroup addressableAssetGroup)
        {
            if (assetType == null)
                return false;

            if (_resultCache.TryGetValue(assetType, out var result))
                return result ^ _invertMatch;

            result = false;
            foreach (var type in _types)
            {
                if (type == assetType)
                {
                    result = true;
                    break;
                }

                if (_matchWithDerivedTypes && assetType.IsSubclassOf(type))
                {
                    result = true;
                    break;
                }
            }

            lock (_resultCacheLocker)
            {
                _resultCache.Add(assetType, result);
            }

            return result ^ _invertMatch;
        }

        public override string GetDescription()
        {
            var result = new StringBuilder();
            var elementCount = 0;
            foreach (var type in _type)
            {
                if (type == null || string.IsNullOrEmpty(type.Name))
                    continue;

                if (elementCount >= 1)
                    result.Append(" || ");

                result.Append(type.Name);
                elementCount++;
            }

            if (result.Length >= 1)
            {
                if (elementCount >= 2)
                {
                    result.Insert(0, "( ");
                    result.Append(" )");
                }

                result.Insert(0, "Type: ");
            }

            if (MatchWithDerivedTypes)
                result.Append(" and derived types");

            if (_invertMatch)
            {
                if (result.Length == 0)
                    return "Not ( Nothing )";

                result.Insert(0, "Not ");
            }

            return result.ToString();
        }
    }
}
