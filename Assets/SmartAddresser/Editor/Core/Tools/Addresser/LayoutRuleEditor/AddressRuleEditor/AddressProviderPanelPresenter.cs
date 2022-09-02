using System;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using StateBasedHistory = SmartAddresser.Editor.Foundation.StateBasedUndo.History;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="AddressProviderPanelView" />.
    /// </summary>
    internal sealed class AddressProviderPanelPresenter : ProviderPanelViewPresenterBase<IAddressProvider>
    {
        public AddressProviderPanelPresenter(AddressProviderPanelView view, AutoIncrementHistory history,
            IAssetSaveService saveService) : base(view, history, saveService)
        {
        }

        protected override Type IgnoreProviderAttributeType => typeof(IgnoreAddressProviderAttribute);
    }
}
