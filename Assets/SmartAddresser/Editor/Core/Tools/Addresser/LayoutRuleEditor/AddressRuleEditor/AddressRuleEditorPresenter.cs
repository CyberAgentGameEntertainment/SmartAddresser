using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="AddressRuleEditorView" />.
    /// </summary>
    internal sealed class AddressRuleEditorPresenter : IDisposable
    {
        private readonly AddressRuleEditorInspectorPresenter _inspectorPresenter;
        private readonly AddressRuleListViewPresenter _listViewPresenter;
        private readonly AddressRuleEditorView _view;
        private bool _didSetupView;

        public AddressRuleEditorPresenter(AddressRuleEditorView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _listViewPresenter = new AddressRuleListViewPresenter(view.ListView, history, saveService);
            _inspectorPresenter = new AddressRuleEditorInspectorPresenter(view.InspectorView, history, saveService);

            SetupViewEventHandlers();
        }

        public void Dispose()
        {
            CleanupViewEventHandler();
            CleanupView();
            _inspectorPresenter.Dispose();
            _listViewPresenter.Dispose();
        }

        public void SetupView(IObservableList<AddressRule> rules)
        {
            CleanupView();

            _listViewPresenter.SetupView(rules);
            if (rules.Count >= 1)
                _inspectorPresenter.SetupView(rules[0]);
            _didSetupView = true;

            var selection = _view.ListView.TreeView.GetSelection();
            ChangeSelectedItem(selection);
        }

        public void CleanupView()
        {
            _listViewPresenter.CleanupView();
            _inspectorPresenter.CleanupView();
            _didSetupView = false;
        }

        private void SetupViewEventHandlers()
        {
            _view.ListView.TreeView.OnSelectionChanged += ChangeSelectedItem;
        }

        private void CleanupViewEventHandler()
        {
            _view.ListView.TreeView.OnSelectionChanged -= ChangeSelectedItem;
        }

        private void ChangeSelectedItem(IList<int> ids)
        {
            if (!_didSetupView)
                return;

            if (ids == null || ids.Count == 0)
            {
                _inspectorPresenter.CleanupView();
                return;
            }

            var id = ids.First();
            if (_view.ListView.TreeView.HasItem(id))
            {
                var item = (AddressRuleListTreeView.Item)_view.ListView.TreeView.GetItem(id);
                _inspectorPresenter.SetupView(item.Rule);
            }
        }
    }
}
