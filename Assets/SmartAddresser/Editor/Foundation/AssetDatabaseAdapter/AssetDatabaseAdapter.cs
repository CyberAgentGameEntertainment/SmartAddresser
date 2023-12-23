using System;
using SmartAddresser.Editor.Core.Tools.Shared;
using UnityEditor;

namespace SmartAddresser.Editor.Foundation.AssetDatabaseAdapter
{
    public sealed class AssetDatabaseAdapter : IAssetDatabaseAdapter
    {
        public string[] GetAllAssetPaths()
        {
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            var projectSettings = SmartAddresserProjectSettings.instance;
            if (projectSettings == null || !projectSettings.SortByAssetPaths)
            {
                return allAssetPaths;
            }
            Array.Sort(allAssetPaths, StringComparer.OrdinalIgnoreCase);
            return allAssetPaths;
        }

        public string GUIDToAssetPath(string guid)
        {
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        public string AssetPathToGUID(string assetPath)
        {
            return AssetDatabase.AssetPathToGUID(assetPath);
        }

        public Type GetMainAssetTypeAtPath(string assetPath)
        {
            return AssetDatabase.GetMainAssetTypeAtPath(assetPath);
        }

        public bool IsValidFolder(string assetPath)
        {
            return AssetDatabase.IsValidFolder(assetPath);
        }
    }
}
