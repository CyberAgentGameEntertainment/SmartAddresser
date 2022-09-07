using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.LabelRuleEditor
{
    internal sealed class LabelRuleEditorInspectorDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Label Rule Editor Inspector";

        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();
        private LabelRuleEditorInspectorPresenter _presenter;
        private LabelRuleEditorInspectorView _view;

        private void OnEnable()
        {
            minSize = new Vector2(200, 100);

            _view = new LabelRuleEditorInspectorView();
            _presenter = new LabelRuleEditorInspectorPresenter(_view, _history, new FakeAssetSaveService());
            var labelRule = new LabelRule();
            _presenter.SetupView(labelRule);
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
                    var labelRule = new LabelRule();
                    _presenter.SetupView(labelRule);
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

        [MenuItem("Window/Smart Addresser/Development/Addresser/Label Rule Editor/Label Rule Editor Inspector")]
        public static void Open()
        {
            GetWindow<LabelRuleEditorInspectorDevelopmentWindow>(WindowName);
        }
    }
}
