using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    /// <summary>
    ///     Filter to pass assets if they are matched by AssetDatabase.FindAssets.
    /// </summary>
    [Serializable]
    [AssetFilter("Find Assets Filter", "Find Assets Filter")]
    public sealed class FindAssetsBasedAssetFilter : AssetFilterBase
    {
        [SerializeField] private string _filter;
        [SerializeField] private DefaultAssetListableProperty _targetFolders = new DefaultAssetListableProperty();

        private readonly List<string> _foundAssetPaths = new List<string>();

        public string Filter
        {
            get => _filter;
            set => _filter = value;
        }

        /// <summary>
        ///     Folders for filtering.
        /// </summary>
        public DefaultAssetListableProperty TargetFolders => _targetFolders;

        public override void SetupForMatching()
        {
            _foundAssetPaths.Clear();
            if (string.IsNullOrWhiteSpace(_filter))
                return;
                    
            var targetFolderPaths = _targetFolders
                .Where(x => x != null)
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();
            var guids = targetFolderPaths.Any()
                ? AssetDatabase.FindAssets(_filter, targetFolderPaths)
                : AssetDatabase.FindAssets(_filter);
            var assetPaths = guids.Select(AssetDatabase.GUIDToAssetPath);
            _foundAssetPaths.AddRange(assetPaths);
        }

        /// <inheritdoc />
        public override bool IsMatch(string assetPath, Type assetType, bool isFolder)
        {
            if (string.IsNullOrEmpty(assetPath))
                return false;

            return _foundAssetPaths.Contains(assetPath);
        }

        public override string GetDescription()
        {
            if (string.IsNullOrWhiteSpace(_filter))
                return string.Empty;
            
            var result = new StringBuilder();
            var elementCount = 0;
            result.Append(_filter);
            foreach (var targetFolder in _targetFolders)
            {
                if (targetFolder == null)
                    continue;

                if (elementCount >= 1)
                    result.Append(", ");

                var path = AssetDatabase.GetAssetPath(targetFolder);
                result.Append(path);
                elementCount++;
            }

            return result.ToString();
        }
    }
}
