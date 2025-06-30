using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups.AssetFilterDrawer
{
    [CustomGUIDrawer(typeof(AddressBasedAssetFilter))]
    internal sealed class AddressBasedAssetFilterDrawer : GUIDrawer<AddressBasedAssetFilter>
    {
        private TextListablePropertyGUI _listablePropertyGUI;

        public override void Setup(object target)
        {
            base.Setup(target);
            _listablePropertyGUI = new TextListablePropertyGUI("Address (Regex)", Target.AddressRegex);
        }

        protected override void GUILayout(AddressBasedAssetFilter target)
        {
            target.MatchWithFolders = EditorGUILayout.Toggle("Match With Folders", target.MatchWithFolders);
            target.Condition =
                (AssetFilterCondition)EditorGUILayout.EnumPopup(
                    ObjectNames.NicifyVariableName(nameof(Target.Condition)), target.Condition);
            _listablePropertyGUI.DoLayout();
        }
    }
}