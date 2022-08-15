using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser
{
    internal sealed class VersionProviderPanelViewDevelopmentWindow
        : ProviderPanelViewDevelopmentWindowBase<IVersionProvider, VersionProviderPanelView,
            VersionProviderPanelViewPresenter>
    {
        private const string WindowName = "[Dev] Version Provider Panel View";

        protected override IVersionProvider CreateInitialProvider()
        {
            return new ConstantVersionProvider();
        }

        protected override VersionProviderPanelView CreteView(ObservableProperty<IVersionProvider> providerProperty)
        {
            return new VersionProviderPanelView(providerProperty);
        }

        protected override VersionProviderPanelViewPresenter CreatePresenter(
            ObservableProperty<IVersionProvider> providerProperty, VersionProviderPanelView view,
            AutoIncrementHistory history, IAssetSaveService assetSaveService)
        {
            return new VersionProviderPanelViewPresenter(providerProperty, view, history, assetSaveService);
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Version Provider Panel View")]
        public static void Open()
        {
            GetWindow<VersionProviderPanelViewDevelopmentWindow>(WindowName);
        }
    }
}
