using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEditor.Build.Utilities;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    internal static class AddressableAssetUtility
    {
        private static readonly HashSet<string> ExcludedExtensions =
            new HashSet<string>(new[] { ".cs", ".js", ".boo", ".exe", ".dll", ".meta" });

        internal static bool IsAssetPathValidForEntry(string assetPath)
        {
            if (ExcludedExtensions.Contains(Path.GetExtension(assetPath)))
                return false;

            if (!assetPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)
                && !IsPathValidPackageAsset(assetPath))
                return false;

            if (assetPath == CommonStrings.UnityEditorResourcePath ||
                assetPath == CommonStrings.UnityDefaultResourcePath ||
                assetPath == CommonStrings.UnityBuiltInExtraPath)
                return false;

            if (assetPath.EndsWith("/Editor", StringComparison.Ordinal) ||
                assetPath.Contains("/Editor/"))
                return false;

            var settings = AddressableAssetSettingsDefaultObject.SettingsExists
                ? AddressableAssetSettingsDefaultObject.Settings
                : null;
            if (settings != null && assetPath.StartsWith(settings.ConfigFolder, StringComparison.Ordinal) ||
                assetPath.StartsWith(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder,
                    StringComparison.Ordinal))
                return false;

            return !ExcludedExtensions.Contains(Path.GetExtension(assetPath));
        }

        internal static bool IsPathValidPackageAsset(string path)
        {
            var splitPath = path.Split('/');

            if (splitPath.Length < 3)
                return false;
            if (splitPath[0] != "Packages")
                return false;
            if (splitPath[2] == "package.json")
                return false;
            return true;
        }
    }
}
