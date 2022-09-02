using Development.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.LabelRuleEditor
{
    internal sealed class LabelProviderPanelViewDevelopmentWindow
        : ProviderPanelViewDevelopmentWindowBase<ILabelProvider, LabelProviderPanelView,
            LabelProviderPanelViewPresenter>
    {
        private const string WindowName = "[Dev] Label Provider Panel View";

        protected override LabelProviderPanelView CreateView()
        {
            return new LabelProviderPanelView();
        }

        protected override LabelProviderPanelViewPresenter CreatePresenter(LabelProviderPanelView view,
            AutoIncrementHistory history, IAssetSaveService assetSaveService)
        {
            return new LabelProviderPanelViewPresenter(view, history, assetSaveService);
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Addresser/Label Rule Editor/Label Provider Panel View")]
        public static void Open()
        {
            GetWindow<LabelProviderPanelViewDevelopmentWindow>(WindowName);
        }
    }
}
