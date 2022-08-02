using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using StateBasedHistory = SmartAddresser.Editor.Foundation.StateBasedUndo.History;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="AddressProviderPanelView" />.
    /// </summary>
    internal sealed class AddressProviderPanelViewPresenter : ProviderPanelViewPresenterBase<IAddressProvider>
    {
        public AddressProviderPanelViewPresenter(ObservableProperty<IAddressProvider> provider,
            AddressProviderPanelView view, AutoIncrementHistory history, IAssetSaveService saveService) : base(provider,
            view, history, saveService)
        {
        }
    }
}
