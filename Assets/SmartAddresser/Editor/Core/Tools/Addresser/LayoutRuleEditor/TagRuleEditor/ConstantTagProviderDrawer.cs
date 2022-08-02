using SmartAddresser.Editor.Core.Models.LayoutRules.TagRules;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.TagRuleEditor
{
    [CustomGUIDrawer(typeof(ConstantTagProvider))]
    public sealed class ConstantTagProviderDrawer : GUIDrawer<ConstantTagProvider>
    {
        protected override void GUILayout(ConstantTagProvider target)
        {
            var tagLabel = ObjectNames.NicifyVariableName(nameof(target.Tag));
            target.Tag = EditorGUILayout.TextField(tagLabel, target.Tag);
        }
    }
}
