using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.EntryRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="AddressRuleEditorListView" />.
    /// </summary>
    internal sealed class AddressRuleEditorListViewPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly Dictionary<string, AddressRuleEditorTreeView.Item> _ruleIdToTreeViewItem =
            new Dictionary<string, AddressRuleEditorTreeView.Item>();

        private readonly AddressRuleEditorListView _view;

        public AddressRuleEditorListViewPresenter(IObservableList<AddressRule> rules, AddressRuleEditorListView view,
            AutoIncrementHistory history, IAssetSaveService saveService)
        {
            _view = view;

            rules.ObservableAdd.Subscribe(x => AddRuleView(x.Value, x.Index)).DisposeWith(_disposables);
            rules.ObservableRemove.Subscribe(x => RemoveRuleView(x.Value)).DisposeWith(_disposables);
            rules.ObservableClear.Subscribe(_ => ClearViews()).DisposeWith(_disposables);

            foreach (var rule in rules)
                AddRuleView(rule);
            _view.TreeView.Reload();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void AddRuleView(AddressRule rule, int index = -1, bool reload = true)
        {
            rule.RefreshAssetGroupDescription();
            rule.RefreshAddressProviderDescription();
            var item = _view.TreeView.AddItem(rule, index);
            _ruleIdToTreeViewItem.Add(rule.Id, item);
            if (reload)
                _view.TreeView.Reload();
        }

        private void RemoveRuleView(AddressRule rule)
        {
            var item = _ruleIdToTreeViewItem[rule.Id];
            _ruleIdToTreeViewItem.Remove(rule.Id);
            _view.TreeView.RemoveItem(item.id);
            _view.TreeView.Reload();
        }

        private void ClearViews()
        {
            _view.TreeView.ClearItems();
            _ruleIdToTreeViewItem.Clear();
        }
    }
}
