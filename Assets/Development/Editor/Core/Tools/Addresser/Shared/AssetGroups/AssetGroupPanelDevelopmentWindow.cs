using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    internal sealed class AssetGroupPanelDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Asset Group Panel";
        private readonly FakeAssetSaveService _assetSaveService = new FakeAssetSaveService();
        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();

        private List<AssetGroup> _groupCollection;
        private AssetGroupPanelPresenter _presenter;
        private Vector2 _scrollPos;
        private AssetGroupPanelView _view;

        private void OnEnable()
        {
            _groupCollection ??= new List<AssetGroup>();

            if (_groupCollection.Count == 0)
                _groupCollection.Add(new AssetGroup());

            _view = new AssetGroupPanelView();
            // Use Address rule type for development window
            _presenter = new AssetGroupPanelPresenter(_view, _history, _assetSaveService, RuleType.Address);
            _presenter.SetupView(_groupCollection, 0);
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

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.ExpandWidth(true)))
            {
                if (GUILayout.Button("Set New Data", EditorStyles.toolbarButton))
                {
                    _groupCollection = new List<AssetGroup> { new AssetGroup() };
                    _presenter.SetupView(_groupCollection, 0);
                }

                if (GUILayout.Button("Clear Data", EditorStyles.toolbarButton))
                    _presenter.CleanupView();
            }

            _view.DoLayout();

            EditorGUILayout.EndScrollView();
        }

        private static bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Addresser/Shared/Asset Group Panel")]
        public static void Open()
        {
            GetWindow<AssetGroupPanelDevelopmentWindow>(WindowName);
        }
    }
}
