using Development.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.VersionRuleEditor
{
    internal sealed class VersionProviderPanelViewDevelopmentWindow
        : ProviderPanelViewDevelopmentWindowBase<IVersionProvider, VersionProviderPanelView,
            VersionProviderPanelViewPresenter>
    {
        private const string WindowName = "[Dev] Version Provider Panel View";

        protected override VersionProviderPanelView CreateView()
        {
            return new VersionProviderPanelView();
        }

        protected override VersionProviderPanelViewPresenter CreatePresenter(VersionProviderPanelView view,
            AutoIncrementHistory history, IAssetSaveService assetSaveService)
        {
            return new VersionProviderPanelViewPresenter(view, history, assetSaveService);
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Addresser/Version Rule Editor/Version Provider Panel View")]
        public static void Open()
        {
            GetWindow<VersionProviderPanelViewDevelopmentWindow>(WindowName);
        }
    }
}
