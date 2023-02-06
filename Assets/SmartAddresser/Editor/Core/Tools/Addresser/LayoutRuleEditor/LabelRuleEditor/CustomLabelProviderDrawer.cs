using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="CustomLabelProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(CustomLabelProvider))]
    public sealed class CustomLabelProviderDrawer : GUIDrawer<CustomLabelProvider>
    {
        protected override void GUILayout(CustomLabelProvider target)
        {
            var labelProviderLabel = ObjectNames.NicifyVariableName(nameof(target.labelProvider));
            target.labelProvider = (LabelProviderAsset)EditorGUILayout.ObjectField(labelProviderLabel,
                target.labelProvider, typeof(LabelProviderAsset), false);
        }
    }
}
