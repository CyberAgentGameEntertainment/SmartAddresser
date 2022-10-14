using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using UnityEditor;
using UnityEditor.AddressableAssets;
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

        private LayoutRuleData _data;
        private LayoutRuleEditorPresenter _presenter;
        private LayoutRuleEditorView _view;

        private string Message { get; } =
            $"{nameof(LayoutRuleData)} cannot be found. Click the following button to create it.";

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);

            var layoutRuleDataPath = AssetDatabase.FindAssets($"t: {nameof(LayoutRuleData)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();

            var data = string.IsNullOrEmpty(layoutRuleDataPath)
                ? null
                : AssetDatabase.LoadAssetAtPath<LayoutRuleData>(layoutRuleDataPath);
            
            Setup(data);
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

            if (_data == null)
            {
                DrawEmptyView();
                return;
            }

            _view.DoLayout();
        }

        private void Setup(LayoutRuleData data)
        {
            _data = data;
            if (data == null)
                return;

            var assetSaveService = new AssetSaveService(data);
            data.LayoutRule.SyncAddressRulesWithAddressableAssetGroups(AddressableAssetSettingsDefaultObject.Settings
                .groups);
            assetSaveService.MarkAsDirty();

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
            _presenter = new LayoutRuleEditorPresenter(_view, _history, assetSaveService);
            _presenter.SetupView(data.LayoutRule);
        }

        private void DrawEmptyView()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(Message);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button($"Create {nameof(LayoutRuleData)}", GUILayout.Width(200)))
            {
                var assetPath = EditorUtility.SaveFilePanelInProject($"Create {nameof(LayoutRuleData)}",
                    $"{nameof(LayoutRuleData)}", "asset", "", "Assets");
                if (!string.IsNullOrEmpty(assetPath))
                {
                    var asset = CreateInstance<LayoutRuleData>();
                    AssetDatabase.CreateAsset(asset, assetPath);
                    Setup(asset);
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
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
