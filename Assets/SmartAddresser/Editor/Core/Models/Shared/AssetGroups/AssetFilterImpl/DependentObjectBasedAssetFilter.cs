using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    /// <summary>
    ///     Filter to pass dependent assets if matches.
    /// </summary>
    [Serializable]
    [AssetFilter("Dependent Object Filter", "Dependent Object Filter")]
    public sealed class DependentObjectBasedAssetFilter : AssetFilterBase
    {
        [SerializeField] private bool _onlyDirectDependencies;
        [SerializeField] private ObjectListableProperty _object = new ObjectListableProperty();

        private HashSet<string> _dependentAssetPaths = new HashSet<string>();
        private bool _hasNullObject;

        /// <summary>
        ///     If true, check only direct dependencies.
        /// </summary>
        public bool OnlyDirectDependencies
        {
            get => _onlyDirectDependencies;
            set => _onlyDirectDependencies = value;
        }

        /// <summary>
        ///     Objects for filtering.
        /// </summary>
        public ObjectListableProperty Object => _object;

        public override void SetupForMatching()
        {
            _dependentAssetPaths.Clear();
            _hasNullObject = false;
            foreach (var obj in _object)
            {
                if (obj == null)
                {
                    _hasNullObject = true;
                    continue;
                }

                var path = AssetDatabase.GetAssetPath(obj);

                // If we already have all the dependencies of 'path', we can skip
                if (!_onlyDirectDependencies && _dependentAssetPaths.Contains(path) == true)
                {
                    continue;
                }

                var dependencies = AssetDatabase.GetDependencies(path, !_onlyDirectDependencies);
                foreach (var dependency in dependencies)
                {
                    _dependentAssetPaths.Add(dependency);
                }
            }
        }

        public override bool Validate(out AssetFilterValidationError error)
        {
            if (_hasNullObject)
            {
                error = new AssetFilterValidationError(this, new[] { "There are null reference objects." });
                return false;
            }

            error = null;
            return true;
        }

        /// <inheritdoc />
        public override bool IsMatch(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            if (string.IsNullOrEmpty(assetPath))
                return false;

            return _dependentAssetPaths.Contains(assetPath);
        }

        public override string GetDescription()
        {
            var result = new StringBuilder();
            var elementCount = 0;
            foreach (var obj in _object)
            {
                if (obj == null) continue;

                if (elementCount >= 1) result.Append(" || ");

                var path = AssetDatabase.GetAssetPath(obj);
                result.Append(Path.GetFileNameWithoutExtension(path));
                elementCount++;
            }

            if (result.Length >= 1)
            {
                if (elementCount >= 2)
                {
                    result.Insert(0, "( ");
                    result.Append(" )");
                }

                result.Insert(0, "Dependent Object: ");
            }

            return result.ToString();
        }
    }
}
