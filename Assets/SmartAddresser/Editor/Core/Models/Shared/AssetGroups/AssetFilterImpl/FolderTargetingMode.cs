using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    public enum FolderTargetingMode
    {
        [InspectorName("Included Assets (Exclude Folders)")]
        IncludedNonFolderAssets = 0,
        Self,
        Both,
    }
}
