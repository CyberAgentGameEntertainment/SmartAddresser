using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor.IMGUI.Controls;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="VersionRuleEditorView" />.
    /// </summary>
    internal sealed class VersionRuleEditorPresenter : IDisposable
    {
        private readonly VersionRuleEditorInspectorPresenter _inspectorPresenter;
        private readonly VersionRuleListPresenter _listPresenter;
        private readonly VersionRuleEditorView _view;
        private bool _didSetupView;

        public VersionRuleEditorPresenter(VersionRuleEditorView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _listPresenter = new VersionRuleListPresenter(view.ListView, history, saveService);
            _inspectorPresenter = new VersionRuleEditorInspectorPresenter(view.InspectorView, history, saveService);

            SetupViewEventHandlers();
        }

        public void Dispose()
        {
            CleanupViewEventHandler();
            CleanupView();
            _inspectorPresenter.Dispose();
            _listPresenter.Dispose();
        }

        public void SetupView(IObservableList<VersionRule> rules)
        {
            CleanupView();

            _listPresenter.SetupView(rules);
            if (rules.Count >= 1)
                _inspectorPresenter.SetupView(rules[0]);
            _didSetupView = true;

            var selection = _view.ListView.TreeView.GetSelection();
            ChangeSelectedItem(selection);
        }

        public void CleanupView()
        {
            _listPresenter.CleanupView();
            _inspectorPresenter.CleanupView();
            _didSetupView = false;
        }

        private void SetupViewEventHandlers()
        {
            _view.ListView.TreeView.OnSelectionChanged += ChangeSelectedItem;
            _view.ListView.TreeView.OnItemRemoved += OnItemRemoved;
        }

        private void CleanupViewEventHandler()
        {
            _view.ListView.TreeView.OnSelectionChanged -= ChangeSelectedItem;
            _view.ListView.TreeView.OnItemRemoved -= OnItemRemoved;
        }

        private void ChangeSelectedItem(IList<int> _)
        {
            if (!_didSetupView)
                return;

            UpdateInspectorView();
        }

        private void OnItemRemoved(TreeViewItem item)
        {
            UpdateInspectorView();
        }

        private void UpdateInspectorView()
        {
            var ids = _view.ListView.TreeView.GetSelection();
            if (ids == null || ids.Count == 0)
            {
                _inspectorPresenter.CleanupView();
                return;
            }

            var id = ids.First();

            if (_view.ListView.TreeView.HasItem(id))
            {
                var item = (VersionRuleListTreeView.Item)_view.ListView.TreeView.GetItem(id);
                _inspectorPresenter.SetupView(item.Rule);
            }
            else
            {
                _inspectorPresenter.CleanupView();
            }
        }
    }
}
