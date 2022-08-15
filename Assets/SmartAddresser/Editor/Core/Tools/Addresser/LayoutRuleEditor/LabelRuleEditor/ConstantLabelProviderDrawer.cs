using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    [CustomGUIDrawer(typeof(ConstantLabelProvider))]
    public sealed class ConstantLabelProviderDrawer : GUIDrawer<ConstantLabelProvider>
    {
        protected override void GUILayout(ConstantLabelProvider target)
        {
            var labelLabel = ObjectNames.NicifyVariableName(nameof(target.Label));
            target.Label = EditorGUILayout.TextField(labelLabel, target.Label);
        }
    }
}
