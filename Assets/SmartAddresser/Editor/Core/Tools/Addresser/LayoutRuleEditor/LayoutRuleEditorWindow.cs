using System.Linq;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation.AddressableAdapter;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using SmartAddresser.Editor.Foundation.TinyRx;
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
        private bool _hasAnyDataChanged;

        private LayoutRuleEditorPresenter _presenter;
        private CompositeDisposable _setupDisposables;
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
            _setupDisposables?.Dispose();
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

        private void OnLostFocus()
        {
            if (focusedWindow is PopupWindow)
                return;

            // If any of data has been changed, apply it to addressable asset system.
            if (_hasAnyDataChanged)
            {
                var projectSettings = SmartAddresserProjectSettings.instance;
                var primaryData = projectSettings.PrimaryData;
                if (primaryData != null)
                {
                    var layoutRules = primaryData.LayoutRules.ToArray();

                    // Validate the layout rule.
                    var validateService = new ValidateAndExportLayoutRuleService(layoutRules);
                    var ruleErrorHandleType = projectSettings.LayoutRuleErrorSettings.HandleType;
                    validateService.Execute(true, ruleErrorHandleType, out _);

                    // Apply the layout rule to the addressable asset system.
                    var versionExpressionParser = new VersionExpressionParserRepository().Load();
                    var assetDatabaseAdapter = new AssetDatabaseAdapter();
                    var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
                    var addressableSettingsAdapter = new AddressableAssetSettingsAdapter(addressableSettings);
                    var applyService = new ApplyLayoutRuleService(layoutRules,
                        versionExpressionParser,
                        addressableSettingsAdapter,
                        assetDatabaseAdapter);
                    applyService.ApplyAll(false);
                }

                _hasAnyDataChanged = false;
            }
        }

        private void Setup()
        {
            _setupDisposables = new CompositeDisposable();
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

            // Observes that any data has changed.
            assetSaveService.IsDirty
                .Subscribe(x =>
                {
                    var projectSettings = SmartAddresserProjectSettings.instance;
                    if (x
                        && !_hasAnyDataChanged
                        && assetSaveService.Asset != null
                        && assetSaveService.Asset == projectSettings.PrimaryData)
                        _hasAnyDataChanged = true;
                })
                .DisposeWith(_setupDisposables);

            // Set up presenter and view.
            _view = new LayoutRuleEditorView(_addressTreeViewState,
                _labelTreeViewState,
                _versionTreeViewState,
                _splitViewState,
                Repaint);
            _presenter = new LayoutRuleEditorPresenter(_view, _history, assetSaveService);
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
