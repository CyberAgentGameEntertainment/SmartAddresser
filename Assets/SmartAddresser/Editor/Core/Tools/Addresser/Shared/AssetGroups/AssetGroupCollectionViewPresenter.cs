using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    /// <summary>
    ///     Presenter for <see cref="AssetGroupCollectionView" />.
    /// </summary>
    internal sealed class AssetGroupCollectionViewPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IObservableList<AssetGroup> _groupCollection;
        private readonly AutoIncrementHistory _history;

        private readonly Dictionary<string, AssetGroupViewPresenter> _presenters =
            new Dictionary<string, AssetGroupViewPresenter>();

        private readonly IAssetSaveService _saveService;
        private readonly AssetGroupCollectionView _view;

        public AssetGroupCollectionViewPresenter(IObservableList<AssetGroup> groupCollection,
            AssetGroupCollectionView view, AutoIncrementHistory history, IAssetSaveService saveService)
        {
            _groupCollection = groupCollection;
            _view = view;
            _history = history;
            _saveService = saveService;

            view.CanPaste += CanPaste;
            view.AddButtonClickedAsObservable.Subscribe(_ => AddGroup()).DisposeWith(_disposables);
            view.PasteMenuExecutedAsObservable.Subscribe(_ => PasteGroup()).DisposeWith(_disposables);
            view.GroupViews.ObservableAdd.Subscribe(x => SetupGroupPresenter(x.Value)).DisposeWith(_disposables);
            view.GroupViews.ObservableRemove.Subscribe(x => DisposeGroupPresenter(x.Key)).DisposeWith(_disposables);

            foreach (var groupView in view.GroupViews.Values)
                SetupGroupPresenter(groupView);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void AddGroup()
        {
            var group = new AssetGroup();
            _history.Register($"Add Group {group.Id}", () =>
            {
                _groupCollection.Add(group);
                _saveService.Save();
            }, () =>
            {
                _groupCollection.Remove(group);
                _saveService.Save();
            });
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

        private bool CanPaste()
        {
            return ObjectCopyBuffer.Type == typeof(AssetGroup);
        }

        private void SetupGroupPresenter(AssetGroupView view)
        {
            var presenter = new AssetGroupViewPresenter(_groupCollection, view, _history, _saveService);
            _presenters.Add(view.Group.Id, presenter);
        }

        private void DisposeGroupPresenter(string groupId)
        {
            var presenter = _presenters[groupId];
            if (presenter != null)
            {
                presenter.Dispose();
                _presenters.Remove(groupId);
            }
        }
    }
}
