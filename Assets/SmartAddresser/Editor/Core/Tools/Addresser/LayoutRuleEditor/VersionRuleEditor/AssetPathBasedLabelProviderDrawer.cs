using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Foundation.CustomDrawers;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="AssetPathBasedVersionProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(AssetPathBasedVersionProvider))]
    public sealed class AssetPathBasedVersionProviderDrawer : AssetPathBasedProviderDrawerBase<AssetPathBasedVersionProvider>
    {
    }
}
