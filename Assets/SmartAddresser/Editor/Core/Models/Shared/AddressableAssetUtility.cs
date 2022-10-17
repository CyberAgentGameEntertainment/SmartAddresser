using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEditor.Build.Utilities;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    /// <summary>
    ///     Class that extracts the necessary parts from AddressableAssetUtility.cs of Addressable Asset System.
    /// </summary>
    internal static class AddressableAssetUtility
    {
        private static readonly HashSet<string> ExcludedExtensions =
            new HashSet<string>(new[] { ".cs", ".js", ".boo", ".exe", ".dll", ".meta" });

        internal static bool IsPathValidForEntry(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            if (!path.StartsWith("assets", StringComparison.OrdinalIgnoreCase) && !IsPathValidPackageAsset(path))
                return false;
            if (path == CommonStrings.UnityEditorResourcePath ||
                path == CommonStrings.UnityDefaultResourcePath ||
                path == CommonStrings.UnityBuiltInExtraPath)
                return false;
            if (path.EndsWith("/Editor") || path.Contains("/Editor/"))
                return false;
            if (path == "Assets")
                return false;
            var settings = AddressableAssetSettingsDefaultObject.SettingsExists
                ? AddressableAssetSettingsDefaultObject.Settings
                : null;
            if (settings != null && path.StartsWith(settings.ConfigFolder) ||
                path.StartsWith(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder))
                return false;
            return !ExcludedExtensions.Contains(Path.GetExtension(path));
        }

        internal static bool IsPathValidPackageAsset(string path)
        {
            var convertPath = path.ToLower().Replace("\\", "/");
            var splitPath = convertPath.Split('/');

            if (splitPath.Length < 3)
                return false;
            if (splitPath[0] != "packages")
                return false;
            if (splitPath[2] == "package.json")
                return false;
            return true;
        }
    }
}
