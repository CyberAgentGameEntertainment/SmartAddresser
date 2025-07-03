using System;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="LabelRuleEditorInspectorView" />.
    /// </summary>
    internal sealed class LabelRuleEditorInspectorPresenter : IDisposable
    {
        private readonly AssetGroupCollectionPanelPresenter _assetGroupCollectionPanelPresenter;
        private readonly LabelProviderPanelViewPresenter _labelProviderPanelPresenter;
        private readonly LabelRuleEditorInspectorView _view;

        public LabelRuleEditorInspectorPresenter(LabelRuleEditorInspectorView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _assetGroupCollectionPanelPresenter =
                new AssetGroupCollectionPanelPresenter(view.GroupCollectionView, history, saveService, RuleType.Label);
            _labelProviderPanelPresenter =
                new LabelProviderPanelViewPresenter(view.LabelProviderPanelView, history, saveService);
        }

        public void Dispose()
        {
            CleanupView();

            _labelProviderPanelPresenter.Dispose();
            _assetGroupCollectionPanelPresenter.Dispose();
        }

        public void SetupView(LabelRule rule)
        {
            CleanupView();

            _assetGroupCollectionPanelPresenter.SetupView(rule.AssetGroups);
            _labelProviderPanelPresenter.SetupView(rule.LabelProvider);
            _view.Enabled = true;
        }

        public void CleanupView()
        {
            _assetGroupCollectionPanelPresenter.CleanupView();
            _labelProviderPanelPresenter.CleanupView();
            _view.Enabled = false;
        }
    }
}
