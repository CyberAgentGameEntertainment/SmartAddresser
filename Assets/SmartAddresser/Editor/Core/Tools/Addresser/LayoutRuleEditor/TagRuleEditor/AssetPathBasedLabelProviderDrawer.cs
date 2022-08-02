using SmartAddresser.Editor.Core.Models.LayoutRules.TagRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Foundation.CustomDrawers;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.TagRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="AssetPathBasedTagProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(AssetPathBasedTagProvider))]
    public sealed class AssetPathBasedTagProviderDrawer : AssetPathBasedProviderDrawerBase<AssetPathBasedTagProvider>
    {
    }
}
