using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.LabelRuleEditor
{
    internal sealed class LabelRuleEditorDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Label Rule Editor";

        [SerializeField] private LabelRuleListTreeView.State _treeViewState;
        [SerializeField] private EditorGUILayoutSplitViewState _splitViewState;

        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();
        private LabelRuleEditorPresenter _presenter;
        private LabelRuleEditorView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);
            if (_treeViewState == null)
                _treeViewState = new LabelRuleListTreeView.State();
            if (_splitViewState == null)
                _splitViewState = new EditorGUILayoutSplitViewState(LayoutDirection.Horizontal, 0.75f);

            _view = new LabelRuleEditorView(_treeViewState, _splitViewState, Repaint);
            _presenter = new LabelRuleEditorPresenter(_view, _history, new FakeAssetSaveService());

            var labelRules = new ObservableList<LabelRule>();
            for (var i = 0; i < 10; i++)
            {
                var labelRule = new LabelRule();
                labelRules.Add(labelRule);
            }

            _presenter.SetupView(labelRules);
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
                    var labelRules = new ObservableList<LabelRule>();
                    for (var i = 0; i < 10; i++)
                    {
                        var labelRule = new LabelRule();
                        labelRules.Add(labelRule);
                    }

                    _presenter.SetupView(labelRules);
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

        [MenuItem("Window/Smart Addresser/Development/Addresser/Label Rule Editor/Label Rule Editor")]
        public static void Open()
        {
            GetWindow<LabelRuleEditorDevelopmentWindow>(WindowName);
        }
    }
}
