// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    /// <summary>
    ///     Filter to pass assets if their extensions match.
    /// </summary>
    [Serializable]
    [AssetFilter("Extension Filter", "Extension Filter")]
    public sealed class ExtensionBasedAssetFilter : AssetFilterBase
    {
        [SerializeField] private StringListableProperty _extension = new StringListableProperty();
        private List<string> _extensions = new List<string>();

        /// <summary>
        ///     Extensions for filtering.
        /// </summary>
        public StringListableProperty Extension => _extension;

        public override void SetupForMatching()
        {
            _extensions.Clear();
            foreach (var extension in _extension)
            {
                if (string.IsNullOrEmpty(extension))
                {
                    continue;
                }

                var ext = extension;
                if (!ext.StartsWith("."))
                {
                    ext = $".{extension}";
                }

                _extensions.Add(ext);
            }
        }

        /// <inheritdoc />
        public override bool IsMatch(string assetPath, Type assetType, bool isFolder)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            foreach (var extension in _extensions)
            {
                // Return true if any of the extensions match.
                if (assetPath.EndsWith(extension, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public override string GetDescription()
        {
            var result = new StringBuilder();
            var elementCount = 0;
            foreach (var extension in _extension)
            {
                if (string.IsNullOrEmpty(extension))
                {
                    continue;
                }

                if (elementCount >= 1)
                {
                    result.Append(" || ");
                }

                result.Append(extension);
                elementCount++;
            }

            if (result.Length >= 1)
            {
                if (elementCount >= 2)
                {
                    result.Insert(0, "( ");
                    result.Append(" )");
                }

                result.Insert(0, "Extension: ");
            }

            return result.ToString();
        }
    }
}
