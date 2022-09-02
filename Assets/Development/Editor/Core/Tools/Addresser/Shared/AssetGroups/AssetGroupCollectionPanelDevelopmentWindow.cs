using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    internal sealed class AssetGroupCollectionPanelDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Asset Group Collection Panel";

        private ObservableList<AssetGroup> _groupCollection;
        private AutoIncrementHistory _history;
        private AssetGroupCollectionPanelPresenter _presenter;
        private Vector2 _scrollPos;
        private AssetGroupCollectionPanelView _view;

        private void OnEnable()
        {
            _history = new AutoIncrementHistory();
            _groupCollection ??= new ObservableList<AssetGroup>();
            _view = new AssetGroupCollectionPanelView();
            _presenter = new AssetGroupCollectionPanelPresenter(_view, _history, new FakeAssetSaveService());
            _presenter.SetupView(_groupCollection);
        }

        private void OnDisable()
        {
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
            
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.ExpandWidth(true)))
            {
                if (GUILayout.Button("Set New Data", EditorStyles.toolbarButton))
                { 
                    _groupCollection = new ObservableList<AssetGroup>();
                    _presenter.SetupView(_groupCollection);
                }

                if (GUILayout.Button("Clear Data", EditorStyles.toolbarButton))
                    _presenter.CleanupView();
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

        [MenuItem("Window/Smart Addresser/Development/Addresser/Shared/Asset Group Collection Panel")]
        public static void Open()
        {
            GetWindow<AssetGroupCollectionPanelDevelopmentWindow>(WindowName);
        }
    }
}
