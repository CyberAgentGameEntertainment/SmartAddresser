using System;
using SmartAddresser.Editor.Core.Models.EntryRules.LabelRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    /// <summary>
    ///     View for the right panel of the Labels tab of the Address Rule Editor.
    /// </summary>
    internal sealed class LabelProviderPanelView : ProviderPanelViewBase<ILabelProvider>
    {
        public LabelProviderPanelView(IReadOnlyObservableProperty<ILabelProvider> provider) : base(provider)
        {
        }

        public override Type IgnoreProviderAttributeType => typeof(IgnoreLabelProviderAttribute);
    }
}
