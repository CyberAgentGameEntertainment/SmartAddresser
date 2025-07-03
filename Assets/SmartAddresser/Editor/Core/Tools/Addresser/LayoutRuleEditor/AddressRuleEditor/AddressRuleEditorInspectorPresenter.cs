using System;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="AddressRuleEditorInspectorView" />.
    /// </summary>
    internal sealed class AddressRuleEditorInspectorPresenter : IDisposable
    {
        private readonly AddressProviderPanelPresenter _addressProviderPanelPresenter;
        private readonly AssetGroupCollectionPanelPresenter _assetGroupCollectionPanelPresenter;
        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();
        private readonly AddressRuleEditorInspectorView _view;

        public AddressRuleEditorInspectorPresenter(AddressRuleEditorInspectorView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _assetGroupCollectionPanelPresenter =
                new AssetGroupCollectionPanelPresenter(view.GroupCollectionView, history, saveService, RuleType.Address);
            _addressProviderPanelPresenter =
                new AddressProviderPanelPresenter(view.AddressProviderPanelView, history, saveService);
        }

        public void Dispose()
        {
            CleanupView();
            
            _addressProviderPanelPresenter.Dispose();
            _assetGroupCollectionPanelPresenter.Dispose();
        }

        public void SetupView(AddressRule rule)
        {
            CleanupView();

            _assetGroupCollectionPanelPresenter.SetupView(rule.AssetGroups);
            _addressProviderPanelPresenter.SetupView(rule.AddressProvider);
            rule.Control.Subscribe(x => _view.Enabled = x).DisposeWith(_setupViewDisposables);
        }

        public void CleanupView()
        {
            _setupViewDisposables.Clear();
            _assetGroupCollectionPanelPresenter.CleanupView();
            _addressProviderPanelPresenter.CleanupView();
            _view.Enabled = false;
        }
    }
}
