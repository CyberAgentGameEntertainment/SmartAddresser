using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     GUI Drawer for <see cref="CustomAddressProvider" />
    /// </summary>
    [CustomGUIDrawer(typeof(CustomAddressProvider))]
    public sealed class CustomAddressProviderDrawer : GUIDrawer<CustomAddressProvider>
    {
        protected override void GUILayout(CustomAddressProvider target)
        {
            var addressProviderLabel = ObjectNames.NicifyVariableName(nameof(target.addressProvider));
            target.addressProvider = (AddressProviderAsset)EditorGUILayout.ObjectField(addressProviderLabel,
                target.addressProvider, typeof(AddressProviderAsset), false);
        }
    }
}
