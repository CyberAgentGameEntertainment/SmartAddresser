using System;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="VersionRuleEditorInspectorView" />.
    /// </summary>
    internal sealed class VersionRuleEditorInspectorPresenter : IDisposable
    {
        private readonly AssetGroupCollectionPanelPresenter _assetGroupCollectionPanelPresenter;
        private readonly VersionProviderPanelViewPresenter _versionProviderPanelPresenter;
        private readonly VersionRuleEditorInspectorView _view;

        public VersionRuleEditorInspectorPresenter(VersionRuleEditorInspectorView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _assetGroupCollectionPanelPresenter =
                new AssetGroupCollectionPanelPresenter(view.GroupCollectionView, history, saveService, RuleType.Version);
            _versionProviderPanelPresenter =
                new VersionProviderPanelViewPresenter(view.VersionProviderPanelView, history, saveService);
        }

        public void Dispose()
        {
            CleanupView();

            _versionProviderPanelPresenter.Dispose();
            _assetGroupCollectionPanelPresenter.Dispose();
        }

        public void SetupView(VersionRule rule)
        {
            CleanupView();

            _assetGroupCollectionPanelPresenter.SetupView(rule.AssetGroups);
            _versionProviderPanelPresenter.SetupView(rule.VersionProvider);
            _view.Enabled = true;
        }

        public void CleanupView()
        {
            _assetGroupCollectionPanelPresenter.CleanupView();
            _versionProviderPanelPresenter.CleanupView();
            _view.Enabled = false;
        }
    }
}
