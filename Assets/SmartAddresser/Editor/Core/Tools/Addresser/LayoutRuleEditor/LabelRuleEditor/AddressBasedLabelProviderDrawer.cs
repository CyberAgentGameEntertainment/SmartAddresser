using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="AddressBasedLabelProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(AddressBasedLabelProvider))]
    public sealed class AddressBasedLabelProviderDrawer : GUIDrawer<AddressBasedLabelProvider>
    {
        protected override void GUILayout(AddressBasedLabelProvider target)
        {
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
