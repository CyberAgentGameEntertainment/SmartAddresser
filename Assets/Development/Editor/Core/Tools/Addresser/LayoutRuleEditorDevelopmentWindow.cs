using Development.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser
{
    internal sealed class LayoutRuleEditorDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Layout Rule Editor";

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
            if (_addressTreeViewState == null)
                _addressTreeViewState = new AddressRuleListTreeView.State();
            if (_labelTreeViewState == null)
                _labelTreeViewState = new LabelRuleListTreeView.State();
            if (_versionTreeViewState == null)
                _versionTreeViewState = new VersionRuleListTreeView.State();
            if (_splitViewState == null)
                _splitViewState = new EditorGUILayoutSplitViewState(LayoutDirection.Horizontal, 0.75f);

            _view = new LayoutRuleEditorView(_addressTreeViewState, _labelTreeViewState, _versionTreeViewState,
                _splitViewState, Repaint);

            var settings = AddressableAssetSettings.Create(null, null, true, false);
            var layoutRuleData = CreateInstance<LayoutRuleData>();
            for (var i = 0; i < 10; i++)
            {
                var group = settings.CreateGroup($"Group-{i:D2}", false, false, true, null);
                var addressRule = new AddressRule(group);
                layoutRuleData.LayoutRule.AddressRules.Add(addressRule);
            }

            var layoutDataLoadService = new FakeLayoutRuleDataRepository();
            layoutDataLoadService.AddData(layoutRuleData);

            var addressableSettingsRepository = new FakeAddressableAssetSettingsRepository();
            addressableSettingsRepository.DataSettingsMap.Add(layoutRuleData, settings);
            _presenter = new LayoutRuleEditorPresenter(_view, _history, new FakeAssetSaveService(),
                addressableSettingsRepository);
            _presenter.SetupView(layoutDataLoadService);
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
                    var layoutRule = new LayoutRule();
                    for (var i = 0; i < 10; i++)
                    {
                        var group = CreateInstance<AddressableAssetGroup>();
                        group.Name = $"Group-{i:D2}";
                        var addressRule = new AddressRule(group);
                        layoutRule.AddressRules.Add(addressRule);
                    }

                    var layoutRuleData = CreateInstance<LayoutRuleData>();
                    layoutRuleData.LayoutRule = layoutRule;
                    _presenter.SetupView(new FakeLayoutRuleDataRepository());
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

        [MenuItem("Window/Smart Addresser/Development/Addresser/Layout Rule Editor")]
        public static void Open()
        {
            GetWindow<LayoutRuleEditorDevelopmentWindow>(WindowName);
        }
    }
}
