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
    ///     Presenter for <see cref="AssetGroupView" />.
    /// </summary>
    internal sealed class AssetGroupViewPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly AssetGroup _group;
        private readonly IList<AssetGroup> _groupCollection;
        private readonly AutoIncrementHistory _history;

        private readonly Dictionary<string, AssetFilterViewPresenter> _presenters =
            new Dictionary<string, AssetFilterViewPresenter>();

        private readonly IAssetSaveService _saveService;

        private readonly AssetGroupView _view;

        public AssetGroupViewPresenter(IList<AssetGroup> groupCollection, AssetGroupView view,
            AutoIncrementHistory history, IAssetSaveService saveService)
        {
            _groupCollection = groupCollection;
            _group = view.Group;
            _view = view;
            _history = history;
            _saveService = saveService;

            view.CanPasteGroup += CanPasteGroup;
            view.CanPasteGroupValues += CanPasteGroupValues;
            view.CanPasteFilter += CanPasteFilter;
            view.NameChangedAsObservable.Subscribe(SetName).DisposeWith(_disposable);
            view.AddFilterButtonClickedAsObservable.Subscribe(_ => AddFilter()).DisposeWith(_disposable);
            view.RemoveGroupMenuExecutedAsObservable.Subscribe(_ => Remove()).DisposeWith(_disposable);
            view.MoveUpMenuExecutedAsObservable.Subscribe(_ => MoveUp()).DisposeWith(_disposable);
            view.MoveDownMenuExecutedAsObservable.Subscribe(_ => MoveDown()).DisposeWith(_disposable);
            view.CopyGroupMenuExecutedAsObservable.Subscribe(_ => Copy()).DisposeWith(_disposable);
            view.PasteGroupMenuExecutedSubject.Subscribe(_ => PasteGroup()).DisposeWith(_disposable);
            view.PasteGroupValuesMenuExecutedSubject.Subscribe(_ => PasteValues()).DisposeWith(_disposable);
            view.PasteFilterMenuExecutedSubject.Subscribe(_ => PasteFilter()).DisposeWith(_disposable);
            view.FilterViews.ObservableAdd.Subscribe(x => SetupFilterPresenter(x.Value)).DisposeWith(_disposable);
            view.FilterViews.ObservableRemove.Subscribe(x => DisposeFilterPresenter(x.Key)).DisposeWith(_disposable);

            foreach (var filterView in view.FilterViews.Values)
                SetupFilterPresenter(filterView);
        }

        public void Dispose()
        {
            foreach (var presenter in _presenters.Values)
                presenter.Dispose();
            _presenters.Clear();

            _disposable.Dispose();
        }

        private void SetName(string name)
        {
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

        private void Remove()
        {
            var index = _groupCollection.IndexOf(_group);
            _history.Register($"Remove Asset Group {_group.Id}", () =>
            {
                _groupCollection.Remove(_group);
                _saveService.Save();
            }, () =>
            {
                _groupCollection.Insert(index, _group);
                _saveService.Save();
            });
        }

        private void MoveUp()
        {
            var index = _groupCollection.IndexOf(_group);
            if (index == 0)
                return;

            _history.Register($"Move Up Group {_group.Id} {index}", () =>
            {
                _groupCollection.RemoveAt(index);
                _groupCollection.Insert(index - 1, _group);
                _saveService.Save();
            }, () =>
            {
                _groupCollection.RemoveAt(index - 1);
                _groupCollection.Insert(index, _group);
                _saveService.Save();
            });
        }

        private void MoveDown()
        {
            var index = _groupCollection.IndexOf(_group);
            if (index == _groupCollection.Count - 1)
                return;

            _history.Register($"Move Down Filter {_group.Id} {index}", () =>
            {
                _groupCollection.RemoveAt(index);
                _groupCollection.Insert(index + 1, _group);
                _saveService.Save();
            }, () =>
            {
                _groupCollection.RemoveAt(index + 1);
                _groupCollection.Insert(index, _group);
                _saveService.Save();
            });
        }

        private void AddFilter()
        {
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

        private void Copy()
        {
            ObjectCopyBuffer.Register(_group);
        }

        private bool CanPasteGroup()
        {
            return ObjectCopyBuffer.Type == typeof(AssetGroup);
        }

        private void PasteGroup()
        {
            var type = ObjectCopyBuffer.Type;
            var json = ObjectCopyBuffer.Json;
            var group = (AssetGroup)Activator.CreateInstance(type);
            group.OverwriteValuesFromJson(json);
            _history.Register($"Paste Group {group.Id}",
                () =>
                {
                    _groupCollection.Add(group);
                    _saveService.Save();
                },
                () =>
                {
                    _groupCollection.Remove(group);
                    _saveService.Save();
                });
        }

        private bool CanPasteGroupValues()
        {
            return ObjectCopyBuffer.Type == typeof(AssetGroup);
        }

        private void PasteValues()
        {
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

        private bool CanPasteFilter()
        {
            return typeof(IAssetFilter).IsAssignableFrom(ObjectCopyBuffer.Type);
        }

        private void PasteFilter()
        {
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

        private void SetupFilterPresenter(AssetFilterView view)
        {
            var presenter = new AssetFilterViewPresenter(_group.Filters, view, _history, _saveService);
            _presenters.Add(view.Filter.Id, presenter);
        }

        private void DisposeFilterPresenter(string filterId)
        {
            var presenter = _presenters[filterId];
            if (presenter != null)
            {
                presenter.Dispose();
                _presenters.Remove(filterId);
            }
        }
    }
}
