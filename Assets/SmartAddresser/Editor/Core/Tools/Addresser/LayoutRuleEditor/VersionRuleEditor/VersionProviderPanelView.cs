using System;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     View for the right panel of the Versions tab of the Address Rule Editor.
    /// </summary>
    internal sealed class VersionProviderPanelView : ProviderPanelViewBase<IVersionProvider>
    {
        public VersionProviderPanelView(IReadOnlyObservableProperty<IVersionProvider> provider) : base(provider)
        {
        }

        public override Type IgnoreProviderAttributeType => typeof(IgnoreLabelProviderAttribute);
    }
}
