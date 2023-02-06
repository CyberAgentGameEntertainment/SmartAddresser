using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="AddressRuleListView" />.
    /// </summary>
    internal sealed class AddressRuleListPresenter : IDisposable
    {
        private readonly IAssetSaveService _assetSaveService;

        private readonly Dictionary<string, CompositeDisposable> _perRuleDisposables =
            new Dictionary<string, CompositeDisposable>();

        private readonly Dictionary<string, AddressRuleListTreeView.Item> _ruleIdToTreeViewItem =
            new Dictionary<string, AddressRuleListTreeView.Item>();

        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();

        private readonly AddressRuleListView _view;

        public AddressRuleListPresenter(AddressRuleListView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _assetSaveService = saveService;
            SetupViewEventHandlers();
        }

        public void Dispose()
        {
            CleanupView();
            CleanupViewEventHandlers();
        }

        public void SetupView(IObservableList<AddressRule> rules)
        {
            CleanupView();

            rules.ObservableAdd.Subscribe(x => AddRuleView(x.Value, x.Index)).DisposeWith(_setupViewDisposables);
            rules.ObservableRemove.Subscribe(x => RemoveRuleView(x.Value)).DisposeWith(_setupViewDisposables);
            rules.ObservableClear.Subscribe(_ => ClearViews()).DisposeWith(_setupViewDisposables);
            foreach (var rule in rules)
                AddRuleView(rule);
            _view.TreeView.Reload();

            #region Local methods

            void AddRuleView(AddressRule rule, int index = -1, bool reload = true)
            {
                var perRuleDisposables = new CompositeDisposable();
                rule.Control
                    .Subscribe(_ =>
                    {
                        _assetSaveService.MarkAsDirty();
                        _assetSaveService.Save();
                    })
                    .DisposeWith(perRuleDisposables);
                var item = _view.TreeView.AddItem(rule, index);
                _ruleIdToTreeViewItem.Add(rule.Id, item);
                _perRuleDisposables.Add(rule.Id, perRuleDisposables);
                if (reload)
                    _view.TreeView.Reload();
            }

            void RemoveRuleView(AddressRule rule)
            {
                var disposables = _perRuleDisposables[rule.Id];
                disposables.Dispose();
                _perRuleDisposables.Remove(rule.Id);
                var item = _ruleIdToTreeViewItem[rule.Id];
                _ruleIdToTreeViewItem.Remove(rule.Id);
                _view.TreeView.RemoveItem(item.id);
                _view.TreeView.Reload();
            }

            void ClearViews()
            {
                _view.TreeView.ClearItems();
                _view.TreeView.Reload();
                foreach (var disposables in _perRuleDisposables.Values)
                    disposables.Dispose();
                _perRuleDisposables.Clear();
                _ruleIdToTreeViewItem.Clear();
            }

            #endregion
        }

        public void CleanupView()
        {
            _setupViewDisposables.Clear();
            _view.TreeView.ClearItems();
            _view.TreeView.Reload();
            _perRuleDisposables.Clear();
            _ruleIdToTreeViewItem.Clear();
        }

        private void SetupViewEventHandlers()
        {
            _view.TreeView.RightClickMenuRequested += OnRightClickMenuRequested;
        }

        private void CleanupViewEventHandlers()
        {
            _view.TreeView.RightClickMenuRequested -= OnRightClickMenuRequested;
        }

        private GenericMenu OnRightClickMenuRequested()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy Asset Groups Description"), false,
                CopySelectedAssetGroupDescriptionAsText);
            menu.AddItem(new GUIContent("Copy Address Rule Description"), false,
                CopySelectedAddressRuleDescriptionAsText);
            return menu;

            #region Local methods

            void CopySelectedAssetGroupDescriptionAsText()
            {
                var selections = _view.TreeView.GetSelection();
                if (selections == null || selections.Count == 0) return;

                var selection = selections[0];
                var item = (AddressRuleListTreeView.Item)_view.TreeView.GetItem(selection);
                GUIUtility.systemCopyBuffer = item.Rule.AssetGroupDescription.Value;
            }

            void CopySelectedAddressRuleDescriptionAsText()
            {
                var selections = _view.TreeView.GetSelection();
                if (selections == null || selections.Count == 0) return;

                var selection = selections[0];
                var item = (AddressRuleListTreeView.Item)_view.TreeView.GetItem(selection);
                GUIUtility.systemCopyBuffer = item.Rule.AddressProviderDescription.Value;
            }

            #endregion
        }
    }
}
