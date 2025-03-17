using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEngine;
using StateBasedHistory = SmartAddresser.Editor.Foundation.StateBasedUndo.History;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    /// <summary>
    ///     Presenter for <see cref="AssetFilterView" />.
    /// </summary>
    internal sealed class AssetFilterViewPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly IAssetFilter _filter;
        private readonly IList<IAssetFilter> _filterCollection;
        private readonly StateBasedHistory _filterHistory;
        private readonly AutoIncrementHistory _history;
        private readonly IAssetSaveService _saveService;
        private readonly AssetFilterView _view;

        private int _mouseButtonClickedCount;

        public AssetFilterViewPresenter(IList<IAssetFilter> filterCollection, AssetFilterView view,
            AutoIncrementHistory history, IAssetSaveService saveService)
        {
            _filterCollection = filterCollection;
            _view = view;
            _filter = view.Filter;
            _history = history;
            _saveService = saveService;
            _filterHistory = new StateBasedHistory(_filter);
            _filterHistory.RegisterSnapshot(_filterHistory.TakeSnapshot());
            _filterHistory.IncrementCurrentGroup();

            view.CanPaste += CanPaste;
            view.CanPasteValues += CanPasteValues;
            view.GetMoveUpByOptions += GetMoveUpByOptions;
            view.GetMoveDownByOptions += GetMoveDownByOptions;
            view.RemoveMenuExecutedAsObservable.Subscribe(_ => Remove()).DisposeWith(_disposable);
            view.MoveUpMenuExecutedAsObservable.Subscribe(_ => MoveUp()).DisposeWith(_disposable);
            view.MoveUpByMenuExecutedAsObservable.Subscribe(MoveUpBy).DisposeWith(_disposable);
            view.MoveDownMenuExecutedAsObservable.Subscribe(_ => MoveDown()).DisposeWith(_disposable);
            view.MoveDownByMenuExecutedAsObservable.Subscribe(MoveDownBy).DisposeWith(_disposable);
            view.CopyMenuExecutedAsObservable.Subscribe(_ => Copy()).DisposeWith(_disposable);
            view.PasteMenuExecutedSubject.Subscribe(_ => Paste()).DisposeWith(_disposable);
            view.PasteValuesMenuExecutedSubject.Subscribe(_ => PasteValues()).DisposeWith(_disposable);
            view.ValueChangedAsObservable.Subscribe(_ => ValueChanged()).DisposeWith(_disposable);
            view.MouseButtonClickedAsObservable.Subscribe(_ => _mouseButtonClickedCount++).DisposeWith(_disposable);
        }

        public void Dispose()
        {
            _view.CanPaste -= CanPaste;
            _view.CanPasteValues -= CanPasteValues;
            _view.GetMoveUpByOptions -= GetMoveUpByOptions;
            _view.GetMoveDownByOptions -= GetMoveDownByOptions;
            _disposable.Dispose();
        }

        private void ValueChanged()
        {
            var registered = _filterHistory.RegisterSnapshot();
            if (!registered)
                return;

            // Set keyboardControl to commandName, so changes to the same control will be processed together.
            // But if the mouse button is clicked, commandName will be changed.
            // As a result, the undo for successive keyboard inputs will be processed at once and for mouse input is undo individually.
            _filterHistory.IncrementCurrentGroup();
            _history.Register($"On Filter Value Changed {GUIUtility.keyboardControl} {_mouseButtonClickedCount}", () =>
            {
                _filterHistory.Redo();
                _saveService.Save();
            }, () =>
            {
                _filterHistory.Undo();
                _saveService.Save();
            });
        }

        private void Remove()
        {
            var index = _filterCollection.IndexOf(_filter);
            _history.Register($"Remove Filter {_filter.Id}",
                () =>
                {
                    _filterCollection.Remove(_filter);
                    _saveService.Save();
                },
                () =>
                {
                    _filterCollection.Insert(index, _filter);
                    _saveService.Save();
                });
        }

        private void MoveUp()
        {
            MoveUpBy(1);
        }

        private void MoveUpBy(int d)
        {
            var index = _filterCollection.IndexOf(_filter);
            if (index - d < 0)
                return;

            _history.Register($"Move Up Filter {_filter.Id} {index} By {d}", () =>
            {
                _filterCollection.RemoveAt(index);
                _filterCollection.Insert(index - d, _filter);
                _saveService.Save();
            }, () =>
            {
                _filterCollection.RemoveAt(index - d);
                _filterCollection.Insert(index, _filter);
                _saveService.Save();
            });
        }

        private void MoveDown()
        {
            MoveDownBy(1);
        }

        private void MoveDownBy(int d)
        {
            var index = _filterCollection.IndexOf(_filter);
            if (index + d >= _filterCollection.Count)
                return;

            _history.Register($"Move Down Filter {_filter.Id} {index} By {d}", () =>
            {
                _filterCollection.RemoveAt(index);
                _filterCollection.Insert(index + d, _filter);
                _saveService.Save();
            }, () =>
            {
                _filterCollection.RemoveAt(index + d);
                _filterCollection.Insert(index, _filter);
                _saveService.Save();
            });
        }

        private void Copy()
        {
            ObjectCopyBuffer.Register(_filter);
        }

        private bool CanPaste()
        {
            return typeof(IAssetFilter).IsAssignableFrom(ObjectCopyBuffer.Type);
        }

        private void Paste()
        {
            var type = ObjectCopyBuffer.Type;
            var json = ObjectCopyBuffer.Json;
            var filter = (IAssetFilter)Activator.CreateInstance(type);
            filter.OverwriteValuesFromJson(json);
            _history.Register($"Paste Filter {filter.Id}",
                () =>
                {
                    _filterCollection.Add(filter);
                    _saveService.Save();
                },
                () =>
                {
                    _filterCollection.Remove(filter);
                    _saveService.Save();
                });
        }

        private bool CanPasteValues()
        {
            return _filter.GetType() == ObjectCopyBuffer.Type;
        }

        private void PasteValues()
        {
            var oldJson = JsonUtility.ToJson(_filter);
            var json = ObjectCopyBuffer.Json;
            _history.Register($"Paste Filter Values {_filter.Id}",
                () =>
                {
                    _filter.OverwriteValuesFromJson(json);
                    _saveService.Save();
                },
                () =>
                {
                    _filter.OverwriteValuesFromJson(oldJson);
                    _saveService.Save();
                });
        }

        private ICollection<int> GetMoveUpByOptions()
        {
            var index = _filterCollection.IndexOf(_filter);
            return index == 0 ? Array.Empty<int>() : Enumerable.Range(1, index).ToArray();
        }

        private ICollection<int> GetMoveDownByOptions()
        {
            var index = _filterCollection.IndexOf(_filter);
            return index == _filterCollection.Count - 1 ? Array.Empty<int>() : Enumerable.Range(1, _filterCollection.Count - index - 1).ToArray();
        }
    }
}
