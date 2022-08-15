using System;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="LabelProviderPanelView" />.
    /// </summary>
    internal sealed class LabelProviderPanelViewPresenter : ProviderPanelViewPresenterBase<ILabelProvider>
    {
        public LabelProviderPanelViewPresenter(LabelProviderPanelView view, AutoIncrementHistory history,
            IAssetSaveService saveService) : base(view, history, saveService)
        {
        }

        protected override Type IgnoreProviderAttributeType => typeof(IgnoreLabelProviderAttribute);
    }
}
