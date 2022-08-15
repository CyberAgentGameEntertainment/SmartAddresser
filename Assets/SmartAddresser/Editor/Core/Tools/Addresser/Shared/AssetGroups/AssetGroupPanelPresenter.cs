using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    /// <summary>
    ///     Presenter for <see cref="AssetGroupPanelView" />.
    /// </summary>
    internal sealed class AssetGroupPanelPresenter : IDisposable
    {
        private readonly Dictionary<string, AssetFilterViewPresenter> _filterPresenters =
            new Dictionary<string, AssetFilterViewPresenter>();

        private readonly AutoIncrementHistory _history;
        private readonly IAssetSaveService _saveService;
        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();
        private readonly AssetGroupPanelView _view;
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();
        private bool _didSetupView;
        private AssetGroup _group;
        private IList<AssetGroup> _groups;

        public AssetGroupPanelPresenter(AssetGroupPanelView view, AutoIncrementHistory history,
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

        public void SetupView(IList<AssetGroup> groups, int groupIndex)
        {
            CleanupView();

            _groups = groups;
            var group = groups[groupIndex];
            _group = group;

            _view.GroupName = group.Name.Value;

            // Observes the models and updates the views.
            group.Name.Subscribe(SetGroupName).DisposeWith(_setupViewDisposables);
            group.Filters.ObservableAdd.Subscribe(x => AddFilterView(x.Value, x.Index))
                .DisposeWith(_setupViewDisposables);
            group.Filters.ObservableRemove.Subscribe(x => RemoveFilterView(x.Value.Id))
                .DisposeWith(_setupViewDisposables);
            group.Filters.ObservableClear.Subscribe(_ => ClearFilterViews()).DisposeWith(_setupViewDisposables);
            group.Filters.ObservableReplace.Subscribe(x => ReplaceFilterView(x.OldValue, x.NewValue, x.Index))
                .DisposeWith(_setupViewDisposables);

            // Handle existing filters.
            foreach (var filter in group.Filters)
            {
                var filterView = _view.AddFilterView(filter);
                var filterPresenter = new AssetFilterViewPresenter(_group.Filters, filterView, _history, _saveService);
                _filterPresenters.Add(filter.Id, filterPresenter);
            }

            _view.Enabled = true;
            _didSetupView = true;

            #region Local methods

            void SetGroupName(string groupName)
            {
                _view.GroupName = groupName;
            }

            void AddFilterView(IAssetFilter filter, int index)
            {
                var filterView = _view.AddFilterView(filter, index);
                var filterPresenter = new AssetFilterViewPresenter(_group.Filters, filterView, _history, _saveService);
                _filterPresenters.Add(filter.Id, filterPresenter);
            }

            void RemoveFilterView(string filterId)
            {
                _filterPresenters[filterId].Dispose();
                _filterPresenters.Remove(filterId);
                _view.RemoveFilterView(filterId);
            }

            void ClearFilterViews()
            {
                foreach (var filterPresenter in _filterPresenters.Values)
                    filterPresenter.Dispose();
                _filterPresenters.Clear();
                _view.ClearFilterViews();
            }

            void ReplaceFilterView(IAssetFilter oldValue, IAssetFilter newValue, int index)
            {
                RemoveFilterView(oldValue.Id);
                AddFilterView(newValue, index);
            }

            #endregion
        }

        public void CleanupView()
        {
            _view.GroupName = null;
            _setupViewDisposables.Clear();
            _view.ClearFilterViews();
            foreach (var presenter in _filterPresenters.Values)
                presenter.Dispose();
            _filterPresenters.Clear();
            _view.Enabled = false;
            _didSetupView = false;
        }

        private void SetupViewEventHandlers()
        {
            _view.CanPasteGroup += CanPasteGroup;
            _view.CanPasteGroupValues += CanPasteGroupValues;
            _view.CanPasteFilter += CanPasteFilter;
            _view.NameChangedAsObservable.Subscribe(SetGroupName).DisposeWith(_viewEventDisposables);
            _view.AddFilterButtonClickedAsObservable.Subscribe(_ => AddFilter()).DisposeWith(_viewEventDisposables);
            _view.RemoveGroupMenuExecutedAsObservable.Subscribe(_ => RemoveGroup()).DisposeWith(_viewEventDisposables);
            _view.MoveUpMenuExecutedAsObservable.Subscribe(_ => MoveUpGroup()).DisposeWith(_viewEventDisposables);
            _view.MoveDownMenuExecutedAsObservable.Subscribe(_ => MoveDownGroup()).DisposeWith(_viewEventDisposables);
            _view.CopyGroupMenuExecutedAsObservable.Subscribe(_ => CopyGroup()).DisposeWith(_viewEventDisposables);
            _view.PasteGroupMenuExecutedSubject.Subscribe(_ => PasteGroup()).DisposeWith(_viewEventDisposables);
            _view.PasteGroupValuesMenuExecutedSubject.Subscribe(_ => PasteGroupValues())
                .DisposeWith(_viewEventDisposables);
            _view.PasteFilterMenuExecutedSubject.Subscribe(_ => PasteFilter()).DisposeWith(_viewEventDisposables);

            #region Local methods

            void SetGroupName(string name)
            {
                if (!_didSetupView)
                    return;

                var oldValue = _group.Name.Value;
                _history.Register($"Set Group Name {_group.Id} {name}", () =>
                {
                    _group.Name.Value = name;
                    _saveService.Save();
                }, () =>
                {
                    _group.Name.Value = oldValue;
                    _saveService.Save();
                });
            }

            void RemoveGroup()
            {
                if (!_didSetupView)
                    return;

                var index = _groups.IndexOf(_group);
                _history.Register($"Remove Asset Group {_group.Id}", () =>
                {
                    _groups.Remove(_group);
                    _saveService.Save();
                }, () =>
                {
                    _groups.Insert(index, _group);
                    _saveService.Save();
                });
            }

            void MoveUpGroup()
            {
                if (!_didSetupView)
                    return;

                var index = _groups.IndexOf(_group);
                if (index == 0)
                    return;

                _history.Register($"Move Up Group {_group.Id} {index}", () =>
                {
                    _groups.RemoveAt(index);
                    _groups.Insert(index - 1, _group);
                    _saveService.Save();
                }, () =>
                {
                    _groups.RemoveAt(index - 1);
                    _groups.Insert(index, _group);
                    _saveService.Save();
                });
            }

            void MoveDownGroup()
            {
                if (!_didSetupView)
                    return;

                var index = _groups.IndexOf(_group);
                if (index == _groups.Count - 1)
                    return;

                _history.Register($"Move Down Filter {_group.Id} {index}", () =>
                {
                    _groups.RemoveAt(index);
                    _groups.Insert(index + 1, _group);
                    _saveService.Save();
                }, () =>
                {
                    _groups.RemoveAt(index + 1);
                    _groups.Insert(index, _group);
                    _saveService.Save();
                });
            }

            void AddFilter()
            {
                if (!_didSetupView)
                    return;

                // Get types of all asset filter.
                var types = TypeCache.GetTypesDerivedFrom<IAssetFilter>()
                    .Where(x => !x.IsAbstract)
                    .Where(x => x.GetCustomAttribute<IgnoreAssetFilterAttribute>() == null);

                // Show filter selection menu.
                var menu = new GenericMenu();
                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<AssetFilterAttribute>();
                    var menuName = attribute == null ? ObjectNames.NicifyVariableName(type.Name) : attribute.MenuName;
                    menu.AddItem(new GUIContent(menuName), false, () =>
                    {
                        var filter = (IAssetFilter)Activator.CreateInstance(type);
                        _history.Register($"Add Filter {filter.Id}", () =>
                        {
                            _group.Filters.Add(filter);
                            _saveService.Save();
                        }, () =>
                        {
                            _group.Filters.Remove(filter);
                            _saveService.Save();
                        });
                    });
                }

                menu.ShowAsContext();
            }

            void CopyGroup()
            {
                if (!_didSetupView)
                    return;

                ObjectCopyBuffer.Register(_group);
            }

            void PasteGroup()
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

            void PasteGroupValues()
            {
                if (!_didSetupView)
                    return;

                var oldJson = JsonUtility.ToJson(_group);
                var json = ObjectCopyBuffer.Json;
                _history.Register($"Paste Group Values {_group.Id}",
                    () =>
                    {
                        _group.OverwriteValuesFromJson(json);
                        _saveService.Save();
                    },
                    () =>
                    {
                        _group.OverwriteValuesFromJson(oldJson);
                        _saveService.Save();
                    });
            }

            void PasteFilter()
            {
                if (!_didSetupView)
                    return;

                var type = ObjectCopyBuffer.Type;
                var json = ObjectCopyBuffer.Json;
                var filter = (IAssetFilter)Activator.CreateInstance(type);
                filter.OverwriteValuesFromJson(json);
                _history.Register($"Paste Filter {filter.Id}",
                    () =>
                    {
                        _group.Filters.Add(filter);
                        _saveService.Save();
                    },
                    () =>
                    {
                        _group.Filters.Remove(filter);
                        _saveService.Save();
                    });
            }

            #endregion
        }

        private void CleanupViewEventHandlers()
        {
            _viewEventDisposables.Clear();
            _view.CanPasteGroup -= CanPasteGroup;
            _view.CanPasteGroupValues -= CanPasteGroupValues;
            _view.CanPasteFilter -= CanPasteFilter;
        }

        private bool CanPasteGroup()
        {
            if (!_didSetupView)
                return false;

            return ObjectCopyBuffer.Type == typeof(AssetGroup);
        }

        private bool CanPasteGroupValues()
        {
            if (!_didSetupView)
                return false;

            return ObjectCopyBuffer.Type == typeof(AssetGroup);
        }

        private bool CanPasteFilter()
        {
            if (!_didSetupView)
                return false;

            return typeof(IAssetFilter).IsAssignableFrom(ObjectCopyBuffer.Type);
        }
    }
}
