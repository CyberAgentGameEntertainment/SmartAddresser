using System;
using SmartAddresser.Editor.Core.Models.EntryRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     View for the right panel of the Addresses tab of the Address Rule Editor.
    /// </summary>
    internal sealed class
        AddressProviderPanelView : ProviderPanelViewBase<IAddressProvider>
    {
        public AddressProviderPanelView(IReadOnlyObservableProperty<IAddressProvider> provider) : base(provider)
        {
        }

        public override Type IgnoreProviderAttributeType => typeof(IgnoreAddressProviderAttribute);
    }
}
