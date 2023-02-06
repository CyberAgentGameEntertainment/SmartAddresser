using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups.AssetFilterDrawer
{
    [CustomGUIDrawer(typeof(CustomAssetFilter))]
    internal sealed class CustomAssetFilterDrawer : GUIDrawer<CustomAssetFilter>
    {
        protected override void GUILayout(CustomAssetFilter target)
        {
            var propertyName = ObjectNames.NicifyVariableName(nameof(target.AssetFilter));
            target.AssetFilter = (AssetFilterAsset)EditorGUILayout.ObjectField(propertyName, target.AssetFilter,
                typeof(AssetFilterAsset), false);
        }
    }
}
