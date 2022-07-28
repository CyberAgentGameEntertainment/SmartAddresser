using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser
{
    internal abstract class ProviderPanelViewDevelopmentWindowBase<TProvider, TView, TPresenter> : EditorWindow
        where TView : ProviderPanelViewBase<TProvider>
        where TPresenter : ProviderPanelViewPresenterBase<TProvider>
    {
        private AutoIncrementHistory _history;
        private TPresenter _presenter;
        private TView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);

            var providerProperty = new ObservableProperty<TProvider>(CreateInitialProvider());
            _view = CreteView(providerProperty);
            _history = new AutoIncrementHistory();
            _presenter = CreatePresenter(providerProperty, _view, _history, new FakeAssetSaveService());
        }

        private void OnDisable()
        {
            _presenter.Dispose();
            _view.Dispose();
        }

        private void OnGUI()
        {
            var e = Event.current;
            if (GetEventAction(e) && e.type == EventType.KeyDown && e.keyCode == KeyCode.Z)
            {
                _history.Undo();
                e.Use();
            }

            if (GetEventAction(e) && e.type == EventType.KeyDown && e.keyCode == KeyCode.Y)
            {
                _history.Redo();
                e.Use();
            }

            _view.DoLayout();
        }

        protected abstract TProvider CreateInitialProvider();

        protected abstract TView CreteView(ObservableProperty<TProvider> providerProperty);

        protected abstract TPresenter CreatePresenter(ObservableProperty<TProvider> providerProperty, TView view,
            AutoIncrementHistory history, IAssetSaveService assetSaveService);

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }
    }
}
