using SmartAddresser.Editor.Core.Models.LayoutRules.TagRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.TagRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="TagProviderPanelView" />.
    /// </summary>
    internal sealed class TagProviderPanelViewPresenter : ProviderPanelViewPresenterBase<ITagProvider>
    {
        public TagProviderPanelViewPresenter(ObservableProperty<ITagProvider> provider,
            TagProviderPanelView view, AutoIncrementHistory history, IAssetSaveService saveService) : base(provider,
            view, history, saveService)
        {
        }
    }
}
