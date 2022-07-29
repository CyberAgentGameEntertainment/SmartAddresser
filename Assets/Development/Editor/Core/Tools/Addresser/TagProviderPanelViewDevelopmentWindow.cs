using SmartAddresser.Editor.Core.Models.EntryRules.TagRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.TagRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser
{
    internal sealed class TagProviderPanelViewDevelopmentWindow
        : ProviderPanelViewDevelopmentWindowBase<ITagProvider, TagProviderPanelView,
            TagProviderPanelViewPresenter>
    {
        private const string WindowName = "[Dev] Tag Provider Panel View";

        protected override ITagProvider CreateInitialProvider()
        {
            return new ConstantTagProvider();
        }

        protected override TagProviderPanelView CreteView(ObservableProperty<ITagProvider> providerProperty)
        {
            return new TagProviderPanelView(providerProperty);
        }

        protected override TagProviderPanelViewPresenter CreatePresenter(
            ObservableProperty<ITagProvider> providerProperty, TagProviderPanelView view,
            AutoIncrementHistory history, IAssetSaveService assetSaveService)
        {
            return new TagProviderPanelViewPresenter(providerProperty, view, history, assetSaveService);
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Tag Provider Panel View")]
        public static void Open()
        {
            GetWindow<TagProviderPanelViewDevelopmentWindow>(WindowName);
        }
    }
}
