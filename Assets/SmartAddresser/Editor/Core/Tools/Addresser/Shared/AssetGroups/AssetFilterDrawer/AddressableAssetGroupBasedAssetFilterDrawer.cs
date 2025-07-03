using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups.AssetFilterDrawer
{
    [CustomGUIDrawer(typeof(AddressableAssetGroupBasedAssetFilter))]
    internal sealed class AddressableAssetGroupBasedAssetFilterDrawer : GUIDrawer<AddressableAssetGroupBasedAssetFilter>
    {
        private AddressableAssetGroupListablePropertyGUI _listablePropertyGUI;

        public override void Setup(object target)
        {
            base.Setup(target);
            _listablePropertyGUI = new AddressableAssetGroupListablePropertyGUI(
                ObjectNames.NicifyVariableName(nameof(Target.Groups)),
                Target.Groups);
        }

        protected override void GUILayout(AddressableAssetGroupBasedAssetFilter target)
        {
            _listablePropertyGUI.DoLayout();
        }
    }
}
