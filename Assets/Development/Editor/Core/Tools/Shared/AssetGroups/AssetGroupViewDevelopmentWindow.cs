using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Shared.AssetGroups
{
    internal sealed class AssetGroupViewDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Asset Group View";
        private CompositeDisposable _disposables;

        private List<AssetGroup> _groupCollection;
        private AutoIncrementHistory _history;
        private AssetGroupViewPresenter _presenter;
        private AssetGroupView _view;
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            _history = new AutoIncrementHistory();
            if (_groupCollection == null)
                _groupCollection = new List<AssetGroup>();

            if (_groupCollection.Count == 0)
                _groupCollection.Add(new AssetGroup());

            var assetGroup = _groupCollection[0];

            _disposables = new CompositeDisposable();
            _view = new AssetGroupView(assetGroup);
            _presenter = new AssetGroupViewPresenter(_groupCollection, _view, _history, new FakeAssetSaveService());
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

        [MenuItem("Window/Smart Addresser/Development/Asset Group View")]
        public static void Open()
        {
            GetWindow<AssetGroupViewDevelopmentWindow>(WindowName);
        }
    }
}
