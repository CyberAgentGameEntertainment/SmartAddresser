using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="AddressableAssetGroupNameBasedVersionProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(AddressableAssetGroupNameBasedVersionProvider))]
    public sealed class AddressableAssetGroupNameBasedVersionProviderDrawer : GUIDrawer<AddressableAssetGroupNameBasedVersionProvider>
    {
        protected override void GUILayout(AddressableAssetGroupNameBasedVersionProvider target)
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
