using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared
{
    public abstract class AssetPathBasedProviderDrawerBase<T> : GUIDrawer<T> where T : AssetPathBasedProvider
    {
        protected override void GUILayout(T target)
        {
            var sourceLabel = ObjectNames.NicifyVariableName(nameof(target.Source));
            target.Source = (PartialAssetPathType)EditorGUILayout.EnumPopup(sourceLabel, target.Source);
            var replaceWithRegexLabel = ObjectNames.NicifyVariableName(nameof(target.ReplaceWithRegex));
            target.ReplaceWithRegex = EditorGUILayout.Toggle(replaceWithRegexLabel, target.ReplaceWithRegex);

            using (new EditorGUI.DisabledScope(!target.ReplaceWithRegex))
            using (new EditorGUI.IndentLevelScope())
            {
                var patternLabel = ObjectNames.NicifyVariableName(nameof(target.Pattern));
                target.Pattern = EditorGUILayout.TextField(patternLabel, target.Pattern);
                var replacementLabel = ObjectNames.NicifyVariableName(nameof(target.Replacement));
                target.Replacement = EditorGUILayout.TextField(replacementLabel, target.Replacement);
            }
        }
    }
}
