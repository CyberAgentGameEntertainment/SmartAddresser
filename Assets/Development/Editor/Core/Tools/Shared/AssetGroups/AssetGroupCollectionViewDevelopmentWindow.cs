using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Shared.AssetGroups
{
    internal sealed class AssetGroupCollectionViewDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Asset Group Collection View";
        private CompositeDisposable _disposables;

        private ObservableList<AssetGroup> _groupCollection;
        private AutoIncrementHistory _history;
        private AssetGroupCollectionViewPresenter _presenter;
        private AssetGroupCollectionView _view;
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            _history = new AutoIncrementHistory();
            if (_groupCollection == null)
                _groupCollection = new ObservableList<AssetGroup>();

            _disposables = new CompositeDisposable();
            _view = new AssetGroupCollectionView(_groupCollection);
            _presenter =
                new AssetGroupCollectionViewPresenter(_groupCollection, _view, _history, new FakeObjectSaveService());
        }

        private void OnDisable()
        {
            _disposables.Dispose();
            _view.Dispose();
            _presenter.Dispose();
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

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            _view.DoLayout();
            
            EditorGUILayout.EndScrollView();
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Asset Group Collection View")]
        public static void Open()
        {
            GetWindow<AssetGroupCollectionViewDevelopmentWindow>(WindowName);
        }
    }
}
