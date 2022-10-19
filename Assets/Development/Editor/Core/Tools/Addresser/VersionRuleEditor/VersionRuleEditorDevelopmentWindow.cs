using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.VersionRuleEditor
{
    internal sealed class VersionRuleEditorDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Version Rule Editor";

        [SerializeField] private VersionRuleListTreeView.State _treeViewState;
        [SerializeField] private EditorGUILayoutSplitViewState _splitViewState;

        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();
        private VersionRuleEditorPresenter _presenter;
        private VersionRuleEditorView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);
            if (_treeViewState == null)
                _treeViewState = new VersionRuleListTreeView.State();
            if (_splitViewState == null)
                _splitViewState = new EditorGUILayoutSplitViewState(LayoutDirection.Horizontal, 0.75f);

            _view = new VersionRuleEditorView(_treeViewState, _splitViewState, Repaint);
            _presenter = new VersionRuleEditorPresenter(_view, _history, new FakeAssetSaveService());

            var versionRules = new ObservableList<VersionRule>();
            for (var i = 0; i < 10; i++)
            {
                var versionRule = new VersionRule();
                versionRules.Add(versionRule);
            }

            _presenter.SetupView(versionRules);
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
                    var versionRules = new ObservableList<VersionRule>();
                    for (var i = 0; i < 10; i++)
                    {
                        var versionRule = new VersionRule();
                        versionRules.Add(versionRule);
                    }

                    _presenter.SetupView(versionRules);
                }

                if (GUILayout.Button("Clear Data", EditorStyles.toolbarButton))
                    _presenter.CleanupView();
            }

            _view.DoLayout();
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Addresser/Version Rule Editor/Version Rule Editor")]
        public static void Open()
        {
            GetWindow<VersionRuleEditorDevelopmentWindow>(WindowName);
        }
    }
}
