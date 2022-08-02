using SmartAddresser.Editor.Core.Models.EntryRules.AddressRules;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressProviderDrawers
{
    /// <summary>
    ///     GUI Drawer for <see cref="AssetPathBasedAddressProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(AssetPathBasedAddressProvider))]
    public sealed class AssetPathBasedAddressProviderDrawer : GUIDrawer<AssetPathBasedAddressProvider>
    {
        protected override void GUILayout(AssetPathBasedAddressProvider target)
        {
            var sourceLabel = ObjectNames.NicifyVariableName(nameof(target.Source));
            target.Source =
                (AssetPathBasedAddressProvider.SourceType)EditorGUILayout.EnumPopup(sourceLabel, target.Source);
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
