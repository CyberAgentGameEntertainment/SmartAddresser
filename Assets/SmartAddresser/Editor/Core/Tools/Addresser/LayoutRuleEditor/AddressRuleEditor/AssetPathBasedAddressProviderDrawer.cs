using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Foundation.CustomDrawers;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="AssetPathBasedAddressProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(AssetPathBasedAddressProvider))]
    public sealed class
        AssetPathBasedAddressProviderDrawer : AssetPathBasedProviderDrawerBase<AssetPathBasedAddressProvider>
    {
    }
}
