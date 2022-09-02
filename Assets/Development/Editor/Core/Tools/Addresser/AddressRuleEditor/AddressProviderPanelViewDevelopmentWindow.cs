using Development.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.AddressRuleEditor
{
    internal sealed class AddressProviderPanelViewDevelopmentWindow
        : ProviderPanelViewDevelopmentWindowBase<IAddressProvider, AddressProviderPanelView,
            AddressProviderPanelPresenter>
    {
        private const string WindowName = "[Dev] Address Provider Panel View";

        protected override AddressProviderPanelView CreateView()
        {
            return new AddressProviderPanelView();
        }

        protected override AddressProviderPanelPresenter CreatePresenter(AddressProviderPanelView view,
            AutoIncrementHistory history, IAssetSaveService assetSaveService)
        {
            return new AddressProviderPanelPresenter(view, history, assetSaveService);
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Addresser/Address Rule Editor/Address Provider Panel View")]
        public static void Open()
        {
            GetWindow<AddressProviderPanelViewDevelopmentWindow>(WindowName);
        }
    }
}
