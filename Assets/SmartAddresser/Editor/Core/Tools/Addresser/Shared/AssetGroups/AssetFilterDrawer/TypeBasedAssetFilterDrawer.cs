using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups.AssetFilterDrawer
{
    [CustomGUIDrawer(typeof(TypeBasedAssetFilter))]
    internal sealed class TypeBasedAssetFilterDrawer : GUIDrawer<TypeBasedAssetFilter>
    {
        private TypeReferenceListablePropertyGUI _listablePropertyGUI;

        public override void Setup(object target)
        {
            base.Setup(target);
            _listablePropertyGUI = new TypeReferenceListablePropertyGUI("Type", Target.Type);
        }

        protected override void GUILayout(TypeBasedAssetFilter target)
        {
            target.InvertMatch =
                EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(Target.InvertMatch)),
                    Target.InvertMatch);
            target.MatchWithDerivedTypes = EditorGUILayout.Toggle(
                ObjectNames.NicifyVariableName(nameof(Target.MatchWithDerivedTypes)), Target.MatchWithDerivedTypes);
            _listablePropertyGUI.DoLayout();
        }
    }
}
