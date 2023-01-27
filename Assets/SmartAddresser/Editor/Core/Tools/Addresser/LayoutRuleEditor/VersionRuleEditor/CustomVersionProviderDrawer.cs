using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="CustomVersionProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(CustomVersionProvider))]
    public sealed class CustomVersionProviderDrawer : GUIDrawer<CustomVersionProvider>
    {
        protected override void GUILayout(CustomVersionProvider target)
        {
            var versionProviderLabel = ObjectNames.NicifyVariableName(nameof(target.versionProvider));
            target.versionProvider = (VersionProviderAsset)EditorGUILayout.ObjectField(versionProviderLabel,
                target.versionProvider, typeof(VersionProviderAsset), false);
        }
    }
}
