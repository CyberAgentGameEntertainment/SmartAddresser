using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;

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
        [SerializeField] private DefaultAssetListableProperty targetFolder = new DefaultAssetListableProperty();

        private readonly List<string> _foundAssetPaths = new List<string>();

        public string Filter
        {
            get => _filter;
            set => _filter = value;
        }

        /// <summary>
        ///     Folders for filtering.
        /// </summary>
        public DefaultAssetListableProperty TargetFolder => targetFolder;

        public override void SetupForMatching()
        {
            _foundAssetPaths.Clear();
            if (string.IsNullOrWhiteSpace(_filter))
                return;

            var targetFolderPaths = targetFolder
                .Where(x => x != null)
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();
            var guids = targetFolderPaths.Any()
                ? AssetDatabase.FindAssets(_filter, targetFolderPaths)
                : AssetDatabase.FindAssets(_filter);
            var assetPaths = guids.Select(AssetDatabase.GUIDToAssetPath);
            _foundAssetPaths.AddRange(assetPaths);
        }

        public override bool Validate(out AssetFilterValidationError error)
        {
            error = null;
            return true;
        }

        /// <inheritdoc />
        public override bool IsMatch(string assetPath, Type assetType, bool isFolder, string address, AddressableAssetGroup addressableAssetGroup)
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
            result.Append($"Filter: {_filter}");
            foreach (var folder in targetFolder)
            {
                if (folder == null)
                    continue;

                result.Append(elementCount == 0 ? " (Folder: " : ", ");

                var path = AssetDatabase.GetAssetPath(folder);
                result.Append(path);
                elementCount++;
            }

            if (elementCount >= 1)
            {
                result.Append(")");
            }

            return result.ToString();
        }
    }
}
