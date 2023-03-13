using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups.AssetFilterDrawer
{
    [CustomGUIDrawer(typeof(FindAssetsBasedAssetFilter))]
    internal sealed class FindAssetsBasedAssetFilterDrawer : GUIDrawer<FindAssetsBasedAssetFilter>
    {
        private DefaultAssetListablePropertyGUI _listablePropertyGUI;

        public override void Setup(object target)
        {
            base.Setup(target);
            _listablePropertyGUI = new DefaultAssetListablePropertyGUI(
                ObjectNames.NicifyVariableName(nameof(Target.TargetFolder)), Target.TargetFolder, false);
        }

        protected override void GUILayout(FindAssetsBasedAssetFilter target)
        {
            var filterDisplayName = ObjectNames.NicifyVariableName(nameof(target.Filter));
            target.Filter = EditorGUILayout.TextField(filterDisplayName, target.Filter);
            _listablePropertyGUI.DoLayout();
        }
    }
}
