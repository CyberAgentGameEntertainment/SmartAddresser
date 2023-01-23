using System;

namespace SmartAddresser.Editor.Foundation.AssetDatabaseAdapter
{
    public interface IAssetDatabaseAdapter
    {
        string[] GetAllAssetPaths();
        
        string GUIDToAssetPath(string guid);
        
        string AssetPathToGUID(string assetPath);

        Type GetMainAssetTypeAtPath(string assetPath);

        bool IsValidFolder(string assetPath);
    }
}
