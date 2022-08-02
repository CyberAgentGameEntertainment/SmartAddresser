using System;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.TagRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.TagRuleEditor
{
    /// <summary>
    ///     View for the right panel of the Tags tab of the Address Rule Editor.
    /// </summary>
    internal sealed class TagProviderPanelView : ProviderPanelViewBase<ITagProvider>
    {
        public TagProviderPanelView(IReadOnlyObservableProperty<ITagProvider> provider) : base(provider)
        {
        }

        public override Type IgnoreProviderAttributeType => typeof(IgnoreLabelProviderAttribute);
    }
}
