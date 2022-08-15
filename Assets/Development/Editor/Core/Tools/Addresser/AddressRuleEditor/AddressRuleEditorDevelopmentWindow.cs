using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.AddressRuleEditor
{
    internal sealed class AddressRuleEditorDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Address Rule Editor";

        [SerializeField] private AddressRuleListTreeView.State _treeViewState;
        [SerializeField] private EditorGUILayoutSplitViewState _splitViewState;

        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();
        private AddressRuleEditorPresenter _presenter;
        private AddressRuleEditorView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);
            if (_treeViewState == null)
                _treeViewState = new AddressRuleListTreeView.State();
            if (_splitViewState == null)
                _splitViewState = new EditorGUILayoutSplitViewState(LayoutDirection.Horizontal, 0.75f);

            _view = new AddressRuleEditorView(_treeViewState, _splitViewState, Repaint);
            _presenter = new AddressRuleEditorPresenter(_view, _history, new FakeAssetSaveService());

            var addressRules = new ObservableList<AddressRule>();
            for (var i = 0; i < 10; i++)
            {
                var group = CreateInstance<AddressableAssetGroup>();
                group.Name = $"Group-{i:D2}";
                var addressRule = new AddressRule(group);
                addressRules.Add(addressRule);
            }

            _presenter.SetupView(addressRules);
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
                    var addressRules = new ObservableList<AddressRule>();
                    for (var i = 0; i < 10; i++)
                    {
                        var group = CreateInstance<AddressableAssetGroup>();
                        group.Name = $"Group-{i:D2}";
                        var addressRule = new AddressRule(group);
                        addressRules.Add(addressRule);
                    }

                    _presenter.SetupView(addressRules);
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

        [MenuItem("Window/Smart Addresser/Development/Addresser/Address Rule Editor/Address Rule Editor")]
        public static void Open()
        {
            GetWindow<AddressRuleEditorDevelopmentWindow>(WindowName);
        }
    }
}
