using System;
using UnityEditor;

namespace SmartAddresser.Editor.Foundation.AssetDatabaseAdapter
{
    public sealed class AssetDatabaseAdapter : IAssetDatabaseAdapter
    {
        public string[] GetAllAssetPaths()
        {
            return AssetDatabase.GetAllAssetPaths();
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
