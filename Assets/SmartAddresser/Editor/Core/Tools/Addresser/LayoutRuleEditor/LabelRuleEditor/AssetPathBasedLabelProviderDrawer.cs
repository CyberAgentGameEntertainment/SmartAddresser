using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Foundation.CustomDrawers;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="AssetPathBasedLabelProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(AssetPathBasedLabelProvider))]
    public sealed class
        AssetPathBasedLabelProviderDrawer : AssetPathBasedProviderDrawerBase<AssetPathBasedLabelProvider>
    {
    }
}
