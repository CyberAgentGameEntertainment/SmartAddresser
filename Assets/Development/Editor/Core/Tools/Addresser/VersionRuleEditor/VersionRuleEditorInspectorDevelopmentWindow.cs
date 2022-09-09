using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.VersionRuleEditor
{
    internal sealed class VersionRuleEditorInspectorDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Version Rule Editor Inspector";

        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();
        private VersionRuleEditorInspectorPresenter _presenter;
        private VersionRuleEditorInspectorView _view;

        private void OnEnable()
        {
            minSize = new Vector2(200, 100);

            _view = new VersionRuleEditorInspectorView();
            _presenter = new VersionRuleEditorInspectorPresenter(_view, _history, new FakeAssetSaveService());
            var versionRule = new VersionRule();
            _presenter.SetupView(versionRule);
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
                    var versionRule = new VersionRule();
                    _presenter.SetupView(versionRule);
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

        [MenuItem("Window/Smart Addresser/Development/Addresser/Version Rule Editor/Version Rule Editor Inspector")]
        public static void Open()
        {
            GetWindow<VersionRuleEditorInspectorDevelopmentWindow>(WindowName);
        }
    }
}
