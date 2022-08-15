using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.AddressRuleEditor
{
    internal sealed class AddressRuleEditorInspectorDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Address Rule Editor Inspector";

        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();
        private AddressRuleEditorInspectorPresenter _presenter;
        private AddressRuleEditorInspectorView _view;

        private void OnEnable()
        {
            minSize = new Vector2(200, 100);

            var group = CreateInstance<AddressableAssetGroup>();
            _view = new AddressRuleEditorInspectorView();
            _presenter = new AddressRuleEditorInspectorPresenter(_view, _history, new FakeAssetSaveService());
            var addressRule = new AddressRule(group);
            _presenter.SetupView(addressRule);
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
                    var group = CreateInstance<AddressableAssetGroup>();
                    var addressRule = new AddressRule(group);
                    _presenter.SetupView(addressRule);
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

        [MenuItem("Window/Smart Addresser/Development/Addresser/Address Rule Editor/Address Rule Editor Inspector")]
        public static void Open()
        {
            GetWindow<AddressRuleEditorInspectorDevelopmentWindow>(WindowName);
        }
    }
}
