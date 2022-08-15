using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    [CustomGUIDrawer(typeof(ConstantVersionProvider))]
    public sealed class ConstantVersionProviderDrawer : GUIDrawer<ConstantVersionProvider>
    {
        protected override void GUILayout(ConstantVersionProvider target)
        {
            var tagLabel = ObjectNames.NicifyVariableName(nameof(target.Version));
            target.Version = EditorGUILayout.TextField(tagLabel, target.Version);
        }
    }
}
