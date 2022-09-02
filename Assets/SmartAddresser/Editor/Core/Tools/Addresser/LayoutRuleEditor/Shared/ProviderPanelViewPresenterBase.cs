using System;
using System.Linq;
using System.Reflection;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;
using StateBasedHistory = SmartAddresser.Editor.Foundation.StateBasedUndo.History;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared
{
    /// <summary>
    ///     Presenter for the <see cref="ProviderPanelViewBase{TProvider}" />.
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    internal abstract class ProviderPanelViewPresenterBase<TProvider> : IDisposable where TProvider : class
    {
        private readonly IAssetSaveService _assetSaveService;
        private readonly AutoIncrementHistory _history;
        private readonly string[] _providerNames;
        private readonly Type[] _providerTypes;
        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();
        private readonly ProviderPanelViewBase<TProvider> _view;
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();
        private bool _didSetupView;
        private int _mouseButtonClickedCount;
        private ObservableProperty<TProvider> _provider;
        private StateBasedHistory _providerHistory;

        public ProviderPanelViewPresenterBase(ProviderPanelViewBase<TProvider> view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _history = history;
            _assetSaveService = saveService;

            var types = TypeCache.GetTypesDerivedFrom<TProvider>()
                .Where(x =>
                {
                    if (x.IsAbstract)
                        return false;
                    var isIgnoreTarget = IgnoreProviderAttributeType == null ||
                                         x.GetCustomAttribute(IgnoreProviderAttributeType) != null;
                    return !isIgnoreTarget;
                })
                .ToArray();

            _providerTypes = new Type[types.Length];
            _providerNames = new string[types.Length];
            for (var index = 0; index < types.Length; index++)
            {
                var type = types[index];
                _providerTypes[index] = type;
                _providerNames[index] = type.Name;
            }

            SetupViewEventHandlers();
        }

        protected abstract Type IgnoreProviderAttributeType { get; }

        public void Dispose()
        {
            CleanupView();
            CleanupViewEventHandlers();
        }

        public void SetupView(ObservableProperty<TProvider> provider)
        {
            CleanupView();

            _provider = provider;

            provider.Subscribe(x => _view.SetProvider(x)).DisposeWith(_setupViewDisposables);

            // Add initial state to history.
            _providerHistory = new StateBasedHistory(provider.Value);
            _providerHistory.RegisterSnapshot(_providerHistory.TakeSnapshot());
            _providerHistory.IncrementCurrentGroup();

            _view.Enabled = true;
            _didSetupView = true;
        }

        public void CleanupView()
        {
            _setupViewDisposables.Clear();
            _view.SetProvider(null);
            _view.Enabled = false;
            _didSetupView = false;
        }

        private void SetupViewEventHandlers()
        {
            _view.ChangeProviderButtonClickedAsObservable.Subscribe(_ => OnChangeProviderButtonClicked())
                .DisposeWith(_viewEventDisposables);
            _view.ProviderValueChangedAsObservable.Subscribe(_ => OnValueChanged())
                .DisposeWith(_viewEventDisposables);
            _view.MouseButtonClickedAsObservable.Subscribe(_ => _mouseButtonClickedCount++)
                .DisposeWith(_viewEventDisposables);

            #region Local methods

            void OnChangeProviderButtonClicked()
            {
                if (!_didSetupView)
                    return;

                var menu = new GenericMenu();

                for (var i = 0; i < _providerNames.Length; i++)
                {
                    var providerName = _providerNames[i];
                    var providerType = _providerTypes[i];
                    menu.AddItem(new GUIContent(providerName), false, () => { ChangeProvider(providerType); });
                }

                menu.ShowAsContext();
            }

            void ChangeProvider(Type type)
            {
                if (!_didSetupView)
                    return;

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

            void OnValueChanged()
            {
                if (!_didSetupView)
                    return;

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

            #endregion
        }

        private void CleanupViewEventHandlers()
        {
            _viewEventDisposables.Clear();
        }
    }
}
