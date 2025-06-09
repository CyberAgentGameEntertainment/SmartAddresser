using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups.AssetFilterDrawer
{
    [CustomGUIDrawer(typeof(ExtensionBasedAssetFilter))]
    internal sealed class ExtensionBasedAssetFilterDrawer : GUIDrawer<ExtensionBasedAssetFilter>
    {
        private TextListablePropertyGUI _listablePropertyGUI;

        public override void Setup(object target)
        {
            base.Setup(target);
            _listablePropertyGUI =
                new TextListablePropertyGUI(ObjectNames.NicifyVariableName(nameof(Target.Extension)),
                    Target.Extension);
        }

        protected override void GUILayout(ExtensionBasedAssetFilter target)
        {
            target.InvertMatch =
                EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(Target.InvertMatch)),
                    Target.InvertMatch);
            _listablePropertyGUI.DoLayout();
        }
    }
}
