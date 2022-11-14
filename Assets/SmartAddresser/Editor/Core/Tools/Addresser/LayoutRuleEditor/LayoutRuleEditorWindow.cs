using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor
{
    internal sealed class LayoutRuleEditorWindow : EditorWindow
    {
        private const string WindowName = "Layout Rule Editor";

        [SerializeField] private AddressRuleListTreeView.State _addressTreeViewState;
        [SerializeField] private LabelRuleListTreeView.State _labelTreeViewState;
        [SerializeField] private VersionRuleListTreeView.State _versionTreeViewState;
        [SerializeField] private EditorGUILayoutSplitViewState _splitViewState;

        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();

        private LayoutRuleEditorPresenter _presenter;
        private LayoutRuleEditorView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);
            Setup();
        }

        private void OnDisable()
        {
            _presenter?.Dispose();
            _view?.Dispose();
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

            _view.DoLayout();
        }

        private void Setup()
        {
            _presenter?.Dispose();
            _view?.Dispose();

            if (_addressTreeViewState == null)
                _addressTreeViewState = new AddressRuleListTreeView.State();
            if (_labelTreeViewState == null)
                _labelTreeViewState = new LabelRuleListTreeView.State();
            if (_versionTreeViewState == null)
                _versionTreeViewState = new VersionRuleListTreeView.State();
            if (_splitViewState == null)
                _splitViewState = new EditorGUILayoutSplitViewState(LayoutDirection.Horizontal, 0.75f);

            var assetSaveService = new AssetSaveService();
            _view = new LayoutRuleEditorView(_addressTreeViewState, _labelTreeViewState, _versionTreeViewState,
                _splitViewState, Repaint);
            _presenter = new LayoutRuleEditorPresenter(_view, _history, assetSaveService,
                new AddressableAssetSettingsRepository());
            _presenter.SetupView(new LayoutRuleDataRepository());
        }

        private static bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Layout Rule Editor")]
        public static void Open()
        {
            GetWindow<LayoutRuleEditorWindow>(WindowName);
        }
    }
}
