using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser
{
    internal sealed class AddressProviderPanelViewDevelopmentWindow
        : ProviderPanelViewDevelopmentWindowBase<IAddressProvider, AddressProviderPanelView,
            AddressProviderPanelViewPresenter>
    {
        private const string WindowName = "[Dev] Address Provider Panel View";

        protected override IAddressProvider CreateInitialProvider()
        {
            return new AssetPathBasedAddressProvider();
        }

        protected override AddressProviderPanelView CreteView(ObservableProperty<IAddressProvider> providerProperty)
        {
            return new AddressProviderPanelView(providerProperty);
        }

        protected override AddressProviderPanelViewPresenter CreatePresenter(
            ObservableProperty<IAddressProvider> providerProperty, AddressProviderPanelView view,
            AutoIncrementHistory history, IAssetSaveService assetSaveService)
        {
            return new AddressProviderPanelViewPresenter(providerProperty, view, history, assetSaveService);
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Address Provider Panel View")]
        public static void Open()
        {
            GetWindow<AddressProviderPanelViewDevelopmentWindow>(WindowName);
        }
    }
}
