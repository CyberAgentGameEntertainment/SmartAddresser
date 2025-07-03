using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    /// <summary>
    ///     Presenter for <see cref="AssetGroupCollectionPanelView" />.
    /// </summary>
    internal sealed class AssetGroupCollectionPanelPresenter : IDisposable
    {
        private readonly Dictionary<string, AssetGroupPanelPresenter> _groupPanelPresenters =
            new Dictionary<string, AssetGroupPanelPresenter>();

        private readonly AutoIncrementHistory _history;
        private readonly RuleType? _ruleType;
        private readonly IAssetSaveService _saveService;
        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();
        private readonly AssetGroupCollectionPanelView _view;
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();
        private bool _didSetupView;
        private IObservableList<AssetGroup> _groups;

        public AssetGroupCollectionPanelPresenter(AssetGroupCollectionPanelView view, AutoIncrementHistory history,
            IAssetSaveService saveService, RuleType? ruleType = null)
        {
            _view = view;
            _history = history;
            _saveService = saveService;
            _ruleType = ruleType;

            SetupViewEventHandlers();
        }

        public void Dispose()
        {
            CleanupView();
            CleanupViewEventHandlers();
        }

        public void SetupView(ObservableList<AssetGroup> groups)
        {
            CleanupView();
            _groups = groups;

            if (_groups.Count == 0)
            {
                var defaultAssetGroup = new AssetGroup
                {
                    Name =
                    {
                        Value = "Default Asset Group"
                    }
                };
                _groups.Add(defaultAssetGroup);
            }

            // Observes the models and updates the views.
            groups.ObservableAdd.Subscribe(x => AddGroupView(x.Value, x.Index)).DisposeWith(_setupViewDisposables);
            groups.ObservableRemove.Subscribe(x => RemoveGroupView(x.Value.Id)).DisposeWith(_setupViewDisposables);
            groups.ObservableClear.Subscribe(x => ClearGroupViews()).DisposeWith(_setupViewDisposables);
            groups.ObservableReplace.Subscribe(x => ReplaceGroupView(x.NewValue, x.OldValue, x.Index))
                .DisposeWith(_setupViewDisposables);

            // Handle existing groups.
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                AddGroupView(group, i);
            }

            _view.Enabled = true;
            _didSetupView = true;

            #region Local methods

            void AddGroupView(AssetGroup group, int index)
            {
                var groupPanelView = _view.AddGroupPanelView(group, index);
                var groupPanelPresenter = new AssetGroupPanelPresenter(groupPanelView, _history, _saveService, _ruleType);
                groupPanelPresenter.SetupView(_groups, index);
                _groupPanelPresenters.Add(group.Id, groupPanelPresenter);
            }

            void RemoveGroupView(string groupId)
            {
                _groupPanelPresenters[groupId].Dispose();
                _groupPanelPresenters.Remove(groupId);
                _view.RemoveGroupPanelView(groupId);
            }

            void ClearGroupViews()
            {
                foreach (var groupPanelPresenter in _groupPanelPresenters.Values)
                    groupPanelPresenter.Dispose();
                _groupPanelPresenters.Clear();
                _view.ClearGroupPanelViews();
            }

            void ReplaceGroupView(AssetGroup oldGroup, AssetGroup newGroup, int index)
            {
                RemoveGroupView(oldGroup.Id);
                AddGroupView(newGroup, index);
            }

            #endregion
        }

        public void CleanupView()
        {
            _setupViewDisposables.Clear();
            foreach (var groupPanelPresenter in _groupPanelPresenters.Values)
                groupPanelPresenter.Dispose();
            _groupPanelPresenters.Clear();
            _view.ClearGroupPanelViews();
            _groups = null;
            _didSetupView = false;
            _view.Enabled = false;
        }

        private void SetupViewEventHandlers()
        {
            _view.AddButtonClickedAsObservable.Subscribe(_ => AddGroupModel()).DisposeWith(_viewEventDisposables);
            _view.PasteMenuExecutedAsObservable.Subscribe(_ => PasteGroupModel()).DisposeWith(_viewEventDisposables);
            _view.CanPaste += CanPasteGroupModel;

            #region Local methods

            void AddGroupModel()
            {
                if (!_didSetupView)
                    return;

                var group = new AssetGroup();
                _history.Register($"Add Group {group.Id}", () =>
                {
                    _groups.Add(group);
                    _saveService.Save();
                }, () =>
                {
                    _groups.Remove(group);
                    _saveService.Save();
                });
            }

            void PasteGroupModel()
            {
                if (!_didSetupView)
                    return;

                var type = ObjectCopyBuffer.Type;
                var json = ObjectCopyBuffer.Json;
                var group = (AssetGroup)Activator.CreateInstance(type);
                group.OverwriteValuesFromJson(json);
                _history.Register($"Paste Group {group.Id}",
                    () =>
                    {
                        _groups.Add(group);
                        _saveService.Save();
                    },
                    () =>
                    {
                        _groups.Remove(group);
                        _saveService.Save();
                    });
            }

            #endregion
        }

        private void CleanupViewEventHandlers()
        {
            _viewEventDisposables.Clear();
            _view.CanPaste -= CanPasteGroupModel;
        }

        private bool CanPasteGroupModel()
        {
            if (!_didSetupView)
                return false;

            return ObjectCopyBuffer.Type == typeof(AssetGroup);
        }
    }
}
