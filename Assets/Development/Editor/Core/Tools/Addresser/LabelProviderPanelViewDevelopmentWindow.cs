using SmartAddresser.Editor.Core.Models.EntryRules.LabelRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser
{
    internal sealed class LabelProviderPanelViewDevelopmentWindow
        : ProviderPanelViewDevelopmentWindowBase<ILabelProvider, LabelProviderPanelView,
            LabelProviderPanelViewPresenter>
    {
        private const string WindowName = "[Dev] Label Provider Panel View";

        protected override ILabelProvider CreateInitialProvider()
        {
            return new AssetPathBasedLabelProvider();
        }

        protected override LabelProviderPanelView CreteView(ObservableProperty<ILabelProvider> providerProperty)
        {
            return new LabelProviderPanelView(providerProperty);
        }

        protected override LabelProviderPanelViewPresenter CreatePresenter(
            ObservableProperty<ILabelProvider> providerProperty, LabelProviderPanelView view,
            AutoIncrementHistory history, IAssetSaveService assetSaveService)
        {
            return new LabelProviderPanelViewPresenter(providerProperty, view, history, assetSaveService);
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Label Provider Panel View")]
        public static void Open()
        {
            GetWindow<LabelProviderPanelViewDevelopmentWindow>(WindowName);
        }
    }
}
