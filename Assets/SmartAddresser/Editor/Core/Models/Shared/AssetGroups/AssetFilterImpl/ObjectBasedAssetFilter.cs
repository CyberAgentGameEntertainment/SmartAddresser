// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    /// <summary>
    ///     Filter to pass assets if matches or contained in the folder.
    /// </summary>
    [Serializable]
    [AssetFilter("Object Filter", "Object Filter")]
    public sealed class ObjectBasedAssetFilter : AssetFilterBase
    {
        [SerializeField] private FolderTargetingMode _folderTargetingMode = FolderTargetingMode.IncludedNonFolderAssets;
        [SerializeField] private ObjectListableProperty _object = new ObjectListableProperty();

        private List<(string assetPath, bool isFolder)> _objectInfoList = new List<(string assetPath, bool isFolder)>();
        private bool _hasNullObject;

        public FolderTargetingMode FolderTargetingMode
        {
            get => _folderTargetingMode;
            set => _folderTargetingMode = value;
        }

        /// <summary>
        ///     Objects for filtering.
        /// </summary>
        public ObjectListableProperty Object => _object;

        public override void SetupForMatching()
        {
            _objectInfoList.Clear();
            _hasNullObject = false;
            foreach (var obj in _object)
            {
                if (obj == null)
                {
                    _hasNullObject = true;
                    continue;
                }

                var isFolder = obj is DefaultAsset;
                var path = AssetDatabase.GetAssetPath(obj);
                _objectInfoList.Add((path, isFolder));
            }
        }
        
        public override bool Validate(out string errorMessage)
        {
            if (_hasNullObject)
            {
                errorMessage = $"[{GetType().Name}] There are null reference objects.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <inheritdoc />
        public override bool IsMatch(string assetPath, Type assetType, bool isFolder)
        {
            if (string.IsNullOrEmpty(assetPath))
                return false;

            var count = _objectInfoList.Count;
            for (var i = 0; i < count; i++)
            {
                var objectAssetPath = _objectInfoList[i].assetPath;
                var objectIsFolder = _objectInfoList[i].isFolder;
                var isSelf = objectAssetPath == assetPath;
                if (objectIsFolder)
                {
                    var isInclusion = !isSelf && !isFolder &&
                                      assetPath.StartsWith(objectAssetPath + "/", StringComparison.Ordinal);
                    switch (FolderTargetingMode)
                    {
                        case FolderTargetingMode.IncludedNonFolderAssets:
                            if (isInclusion)
                                return true;
                            break;
                        case FolderTargetingMode.Self:
                            if (isSelf && isFolder)
                                return true;
                            break;
                        case FolderTargetingMode.Both:
                            if (isInclusion || (isSelf && isFolder))
                                return true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    if (isSelf)
                        return true;
                }
            }

            return false;
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

                result.Insert(0, "Object: ");
            }

            return result.ToString();
        }
    }
}
