using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="AddressBasedVersionProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(AddressBasedVersionProvider))]
    public sealed class AddressBasedVersionProviderDrawer : GUIDrawer<AddressBasedVersionProvider>
    {
        protected override void GUILayout(AddressBasedVersionProvider target)
        {
            var useFullAddressLabel = ObjectNames.NicifyVariableName(nameof(target.UseFullAddress));
            target.UseFullAddress = EditorGUILayout.Toggle(useFullAddressLabel, target.UseFullAddress);
            
            var replaceWithRegexLabel = ObjectNames.NicifyVariableName(nameof(target.ReplaceWithRegex));
            target.ReplaceWithRegex = EditorGUILayout.Toggle(replaceWithRegexLabel, target.ReplaceWithRegex);

            GUI.enabled = target.ReplaceWithRegex;
            using (new EditorGUI.IndentLevelScope())
            {
                var patternLabel = ObjectNames.NicifyVariableName(nameof(target.Pattern));
                target.Pattern = EditorGUILayout.TextField(patternLabel, target.Pattern);
                var replacementLabel = ObjectNames.NicifyVariableName(nameof(target.Replacement));
                target.Replacement = EditorGUILayout.TextField(replacementLabel, target.Replacement);
            }

            GUI.enabled = true;
        }
    }
}