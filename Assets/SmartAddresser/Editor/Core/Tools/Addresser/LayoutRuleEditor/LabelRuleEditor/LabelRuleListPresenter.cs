using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="LabelRuleListView" />.
    /// </summary>
    internal sealed class LabelRuleListPresenter : IDisposable
    {
        private readonly AutoIncrementHistory _history;

        private readonly Dictionary<string, LabelRuleListTreeView.Item> _ruleIdToTreeViewItem =
            new Dictionary<string, LabelRuleListTreeView.Item>();

        private readonly IAssetSaveService _saveService;
        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();
        private readonly LabelRuleListView _view;
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();
        private bool _didSetupView;
        private IObservableList<LabelRule> _rules;

        public LabelRuleListPresenter(LabelRuleListView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _history = history;
            _saveService = saveService;

            SetupViewEventHandlers();
        }

        public void Dispose()
        {
            CleanupView();
            CleanupViewEventHandlers();
        }

        public void SetupView(IObservableList<LabelRule> rules)
        {
            CleanupView();

            _rules = rules;

            rules.ObservableAdd.Subscribe(x => AddRuleView(x.Value, x.Index)).DisposeWith(_setupViewDisposables);
            rules.ObservableRemove.Subscribe(x => RemoveRuleView(x.Value)).DisposeWith(_setupViewDisposables);
            rules.ObservableClear.Subscribe(_ => ClearViews()).DisposeWith(_setupViewDisposables);
            foreach (var rule in rules)
                AddRuleView(rule);
            _view.TreeView.Reload();

            _didSetupView = true;

            #region Local methods

            void AddRuleView(LabelRule rule, int index = -1, bool reload = true)
            {
                var item = _view.TreeView.AddItem(rule, index);
                _ruleIdToTreeViewItem.Add(rule.Id, item);
                if (reload)
                    _view.TreeView.Reload();
            }

            void RemoveRuleView(LabelRule rule)
            {
                var item = _ruleIdToTreeViewItem[rule.Id];
                _ruleIdToTreeViewItem.Remove(rule.Id);
                _view.TreeView.RemoveItem(item.id);
                _view.TreeView.Reload();
            }

            void ClearViews()
            {
                _view.TreeView.ClearItems();
                _view.TreeView.Reload();
                _ruleIdToTreeViewItem.Clear();
            }

            #endregion
        }

        public void CleanupView()
        {
            _setupViewDisposables.Clear();
            _view.TreeView.ClearItems();
            _view.TreeView.Reload();
            _ruleIdToTreeViewItem.Clear();

            _didSetupView = false;
        }

        private void SetupViewEventHandlers()
        {
            _view.AddButtonClickedAsObservable.Subscribe(x => AddRule()).DisposeWith(_viewEventDisposables);
            _view.TreeView.RightClickMenuRequested += OnRightClickMenuRequested;
            _view.TreeView
                .ItemIndexChangedAsObservable
                .Subscribe(x => MoveRule(x.item.Rule, x.newIndex))
                .DisposeWith(_viewEventDisposables);
        }

        private void CleanupViewEventHandlers()
        {
            _view.TreeView.RightClickMenuRequested -= OnRightClickMenuRequested;
            _viewEventDisposables.Clear();
        }

        private void AddRule()
        {
            if (!_didSetupView)
                return;

            var rule = new LabelRule();
            _history.Register($"Add Label Rule {rule.Id}", () =>
            {
                _rules.Add(rule);
                _saveService.Save();
            }, () =>
            {
                _rules.Remove(rule);
                _saveService.Save();
            });
        }

        private void RemoveRule(LabelRule rule)
        {
            if (!_didSetupView)
                return;

            _history.Register($"Remove Label Rule {rule.Id}", () =>
            {
                _rules.Remove(rule);
                _saveService.Save();
            }, () =>
            {
                _rules.Add(rule);
                _saveService.Save();
            });
        }

        private void MoveRule(LabelRule rule, int newIndex)
        {
            if (!_didSetupView)
                return;

            var oldIndex = _rules.IndexOf(rule);
            if (oldIndex == newIndex)
                return;
            
            // To undo all the changes in the same frame, use Time.frameCount to actionTypeId.
            _history.Register($"Move Label Rule {Time.frameCount}", () =>
            {
                _rules.RemoveAt(oldIndex);
                _rules.Insert(newIndex, rule);
                _saveService.Save();
            }, () =>
            {
                _rules.RemoveAt(newIndex);
                _rules.Insert(oldIndex, rule);
                _saveService.Save();
            });
        }

        private void RemoveSelectedItems()
        {
            var rules = _view.TreeView
                .GetSelection()
                .Where(x => _view.TreeView.HasItem(x))
                .Select(x =>
                {
                    var item = (LabelRuleListTreeView.Item)_view.TreeView.GetItem(x);
                    return item.Rule;
                })
                .ToArray();

            foreach (var rule in rules)
                RemoveRule(rule);
        }

        private GenericMenu OnRightClickMenuRequested()
        {
            var treeView = _view.TreeView;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create"), false, AddRule);
            if (treeView.HasSelection())
                menu.AddItem(new GUIContent("Remove"), false, RemoveSelectedItems);
            else
                menu.AddDisabledItem(new GUIContent("Remove"));
            menu.AddItem(new GUIContent("Copy Asset Groups Description"), false,
                CopySelectedAssetGroupDescriptionAsText);
            menu.AddItem(new GUIContent("Copy Label Rule Description"), false,
                CopySelectedLabelRuleDescriptionAsText);
            return menu;
            
            #region Local methods

            void CopySelectedAssetGroupDescriptionAsText()
            {
                var selections = _view.TreeView.GetSelection();
                if (selections == null || selections.Count == 0) return;

                var selection = selections[0];
                var item = (LabelRuleListTreeView.Item)_view.TreeView.GetItem(selection);
                GUIUtility.systemCopyBuffer = item.Rule.AssetGroupDescription.Value;
            }

            void CopySelectedLabelRuleDescriptionAsText()
            {
                var selections = _view.TreeView.GetSelection();
                if (selections == null || selections.Count == 0) return;

                var selection = selections[0];
                var item = (LabelRuleListTreeView.Item)_view.TreeView.GetItem(selection);
                GUIUtility.systemCopyBuffer = item.Rule.LabelProviderDescription.Value;
            }

            #endregion
        }
    }
}
