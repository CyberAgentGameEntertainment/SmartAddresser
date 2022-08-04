using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="VersionProviderPanelView" />.
    /// </summary>
    internal sealed class VersionProviderPanelViewPresenter : ProviderPanelViewPresenterBase<IVersionProvider>
    {
        public VersionProviderPanelViewPresenter(ObservableProperty<IVersionProvider> provider,
            VersionProviderPanelView view, AutoIncrementHistory history, IAssetSaveService saveService) : base(provider,
            view, history, saveService)
        {
        }
    }
}
