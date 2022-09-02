using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.Shared
{
    internal abstract class ProviderPanelViewDevelopmentWindowBase<TProvider, TView, TPresenter> : EditorWindow
        where TView : ProviderPanelViewBase<TProvider>
        where TPresenter : ProviderPanelViewPresenterBase<TProvider>
        where TProvider : class
    {
        private AutoIncrementHistory _history;
        private TPresenter _presenter;
        private TView _view;

        private void OnEnable()
        {
            minSize = new Vector2(200, 200);

            var providerProperty = new ObservableProperty<TProvider>();
            _view = CreateView();
            _history = new AutoIncrementHistory();
            _presenter = CreatePresenter(_view, _history, new FakeAssetSaveService());
            _presenter.SetupView(providerProperty);
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

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.ExpandWidth(true)))
            {
                if (GUILayout.Button("Set New Data", EditorStyles.toolbarButton))
                {
                    var providerProperty = new ObservableProperty<TProvider>();
                    _presenter.SetupView(providerProperty);
                }

                if (GUILayout.Button("Clear Data", EditorStyles.toolbarButton))
                    _presenter.CleanupView();
            }

            _view.DoLayout();
        }

        protected abstract TView CreateView();

        protected abstract TPresenter CreatePresenter(TView view, AutoIncrementHistory history,
            IAssetSaveService assetSaveService);

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
