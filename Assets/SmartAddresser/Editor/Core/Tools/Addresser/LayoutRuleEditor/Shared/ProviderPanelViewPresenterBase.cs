using System;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEngine;
using StateBasedHistory = SmartAddresser.Editor.Foundation.StateBasedUndo.History;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared
{
    internal abstract class ProviderPanelViewPresenterBase<TProvider> : IDisposable
    {
        private readonly IAssetSaveService _assetSaveService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly AutoIncrementHistory _history;
        private readonly ObservableProperty<TProvider> _provider;
        private int _mouseButtonClickedCount;
        private StateBasedHistory _providerHistory;

        public ProviderPanelViewPresenterBase(ObservableProperty<TProvider> provider,
            ProviderPanelViewBase<TProvider> view, AutoIncrementHistory history, IAssetSaveService saveService)
        {
            _provider = provider;
            _history = history;
            _assetSaveService = saveService;

            view.ProviderTypeChangedAsObservable.Subscribe(ChangeProvider).DisposeWith(_disposables);
            view.ProviderValueChangedAsObservable.Subscribe(_ => OnValueChanged()).DisposeWith(_disposables);
            view.MouseButtonClickedAsObservable.Subscribe(_ => _mouseButtonClickedCount++).DisposeWith(_disposables);

            _providerHistory = new StateBasedHistory(provider.Value);
            _providerHistory.RegisterSnapshot(_providerHistory.TakeSnapshot());
            _providerHistory.IncrementCurrentGroup();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void ChangeProvider(Type type)
        {
            var oldInstance = _provider.Value;
            var oldHistory = _providerHistory;
            var newInstance = (TProvider)Activator.CreateInstance(type);
            var newHistory = new StateBasedHistory(newInstance);
            newHistory.RegisterSnapshot(newHistory.TakeSnapshot());
            newHistory.IncrementCurrentGroup();

            _history.Register($"Change Provider {type}", () =>
            {
                _provider.Value = newInstance;
                _providerHistory = newHistory;
                _assetSaveService.Save();
            }, () =>
            {
                _provider.Value = oldInstance;
                _providerHistory = oldHistory;
                _assetSaveService.Save();
            });
        }

        private void OnValueChanged()
        {
            var registered = _providerHistory.RegisterSnapshot();
            if (!registered)
                return;

            // Set keyboardControl to commandName, so changes to the same control will be processed together.
            // But if the mouse button is clicked, commandName will be changed.
            // As a result, the undo for successive keyboard inputs will be processed at once and for mouse input is undo individually.
            _providerHistory.IncrementCurrentGroup();
            _history.Register(
                $"On Provider Value Changed {typeof(TProvider).Name} {GUIUtility.keyboardControl} {_mouseButtonClickedCount}",
                () =>
                {
                    _providerHistory.Redo();
                    _assetSaveService.Save();
                }, () =>
                {
                    _providerHistory.Undo();
                    _assetSaveService.Save();
                });
        }
    }
}
