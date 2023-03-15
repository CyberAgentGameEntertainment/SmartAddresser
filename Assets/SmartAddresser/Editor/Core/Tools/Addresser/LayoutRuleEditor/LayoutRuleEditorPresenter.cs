using System;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.SettingsEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation.AddressableAdapter;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="LayoutRuleEditorView" />.
    /// </summary>
    internal sealed class LayoutRuleEditorPresenter : IDisposable
    {
        private readonly AddressRuleEditorPresenter _addressRuleEditorPresenter;
        private readonly IAssetSaveService _assetSaveService;
        private readonly ObservableProperty<LayoutRuleData> _editingData = new ObservableProperty<LayoutRuleData>();
        private readonly LabelRuleEditorPresenter _labelRuleEditorPresenter;
        private readonly SettingsEditorPresenter _settingsEditorPresenter;
        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();
        private readonly VersionRuleEditorPresenter _versionRuleEditorPresenter;
        private readonly LayoutRuleEditorView _view;
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();
        private AddressableAssetSettings _addressableAssetSettings;

        private ILayoutRuleDataRepository _dataRepository;
        private bool _didSetupView;

        public LayoutRuleEditorPresenter(LayoutRuleEditorView view, AutoIncrementHistory history,
            IAssetSaveService assetSaveService)
        {
            _view = view;
            _addressRuleEditorPresenter =
                new AddressRuleEditorPresenter(view.AddressRuleEditorView, history, assetSaveService);
            _labelRuleEditorPresenter =
                new LabelRuleEditorPresenter(view.LabelRuleEditorView, history, assetSaveService);
            _versionRuleEditorPresenter =
                new VersionRuleEditorPresenter(view.VersionRuleEditorView, history, assetSaveService);
            _settingsEditorPresenter =
                new SettingsEditorPresenter(view.SettingsEditorView, history, assetSaveService);
            _assetSaveService = assetSaveService;

            SetupViewEventHandlers();
        }

        public IReadOnlyObservableProperty<LayoutRuleData> EditingData => _editingData;

        public void Dispose()
        {
            _addressRuleEditorPresenter.Dispose();
            _labelRuleEditorPresenter.Dispose();
            _versionRuleEditorPresenter.Dispose();
            _settingsEditorPresenter.Dispose();
            CleanupView();
            CleanupViewEventHandlers();
            _editingData.Dispose();
        }

        public void SetupView(ILayoutRuleDataRepository dataRepository)
        {
            _setupViewDisposables.Clear();
            _dataRepository = dataRepository;
            dataRepository.EditingData
                .Subscribe(SetupActiveView)
                .DisposeWith(_setupViewDisposables);
        }

        private void SetupActiveView(LayoutRuleData data)
        {
            CleanupActiveView();

            if (data == null)
            {
                _view.ActiveMode.Value = LayoutRuleEditorView.Mode.Create;
                _editingData.Value = null;
                _assetSaveService.SetAsset(data);
                return;
            }

            if (data == _editingData.Value)
                return;

            var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
            _addressableAssetSettings = addressableAssetSettings;
            if (addressableAssetSettings == null)
            {
                _view.ActiveMode.Value = LayoutRuleEditorView.Mode.NoAddressableSettings;
                _editingData.Value = null;
                _assetSaveService.SetAsset(data);
                return;
            }

            _editingData.Value = data;
            _assetSaveService.SetAsset(data);

            if (data.LayoutRule.SyncAddressRulesWithAddressableAssetGroups(addressableAssetSettings.groups))
                _assetSaveService.MarkAsDirty();

            _addressRuleEditorPresenter.SetupView(data.LayoutRule.AddressRules);
            _labelRuleEditorPresenter.SetupView(data.LayoutRule.LabelRules);
            _versionRuleEditorPresenter.SetupView(data.LayoutRule.VersionRules);
            _settingsEditorPresenter.SetupView(data.LayoutRule.Settings);
            _view.ActiveMode.Value = LayoutRuleEditorView.Mode.Edit;
            _view.ActiveAssetName.Value = data.name;

            addressableAssetSettings.OnModification += OnAddressableAssetSettingsModified;

            _didSetupView = true;
        }

        private void OnAddressableAssetSettingsModified(AddressableAssetSettings settings,
            AddressableAssetSettings.ModificationEvent e, object obj)
        {
            if (e == AddressableAssetSettings.ModificationEvent.GroupAdded
                || e == AddressableAssetSettings.ModificationEvent.GroupMoved
                || e == AddressableAssetSettings.ModificationEvent.GroupRemoved
                || e == AddressableAssetSettings.ModificationEvent.GroupRenamed)
                // If the addressable asset group is changed, reload.
                SetupActiveView(_editingData.Value);
        }

        public void CleanupView()
        {
            _setupViewDisposables.Clear();
            CleanupActiveView();
            _dataRepository = null;
        }

        private void CleanupActiveView()
        {
            _addressRuleEditorPresenter.CleanupView();
            _labelRuleEditorPresenter.CleanupView();
            _versionRuleEditorPresenter.CleanupView();
            _settingsEditorPresenter.CleanupView();
            if (_addressableAssetSettings != null)
                _addressableAssetSettings.OnModification -= OnAddressableAssetSettingsModified;
            _addressableAssetSettings = null;
            _view.ActiveMode.Value = LayoutRuleEditorView.Mode.Create;
            _editingData.Value = null;
            _didSetupView = false;
        }

        private void SetupViewEventHandlers()
        {
            _view.BeforeLayoutAsObservable
                .Subscribe(_ => OnBeforeLayout())
                .DisposeWith(_viewEventDisposables);

            _view.AssetSelectButtonClickedAsObservable
                .Subscribe(x => OnAssetSelectButtonClicked())
                .DisposeWith(_viewEventDisposables);

            _view.MenuButtonClickedAsObservable
                .Subscribe(x => OnMenuButtonClicked())
                .DisposeWith(_viewEventDisposables);

            _view.CreateButtonClickedAsObservable
                .Subscribe(_ => OnCreateButtonClicked())
                .DisposeWith(_viewEventDisposables);

            #region Local methods

            void OnBeforeLayout()
            {
                // If the LayoutRuleData asset was deleted, set the first data instead.
                if (_editingData.Value == null)
                {
                    var data = _dataRepository.LoadAll().FirstOrDefault();
                    if (data == null)
                    {
                        CleanupActiveView();
                    }
                    else
                    {
                        SetupActiveView(data);
                        _dataRepository.SetEditingData(data);
                    }
                }

                var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
                // If the AddressableAssetSettings asset was deleted, reload.
                if (_view.ActiveMode.Value != LayoutRuleEditorView.Mode.NoAddressableSettings
                    && addressableAssetSettings == null)
                    SetupActiveView(_editingData.Value);

                // If the AddressableAssetSettings asset was created, reload.
                if (_view.ActiveMode.Value == LayoutRuleEditorView.Mode.NoAddressableSettings
                    && addressableAssetSettings != null)
                    SetupActiveView(_editingData.Value);
            }

            void OnAssetSelectButtonClicked()
            {
                if (!_didSetupView)
                    return;

                var menu = new GenericMenu();

                var sourceAssets = _dataRepository.LoadAll().ToList();
                var sourceAssetNames = sourceAssets.Select(y => y.name).ToArray();
                var activeSourceAssetIndex = sourceAssets.IndexOf(_editingData.Value);
                if (activeSourceAssetIndex == -1)
                    activeSourceAssetIndex = 0;

                for (var i = 0; i < sourceAssetNames.Length; i++)
                {
                    var menuName = $"{sourceAssetNames[i]}";
                    var isActive = activeSourceAssetIndex == i;
                    var idx = i;
                    menu.AddItem(new GUIContent(menuName), isActive, () =>
                    {
                        var asset = sourceAssets[idx];
                        SetupActiveView(asset);
                        _dataRepository.SetEditingData(asset);
                    });
                }

                menu.ShowAsContext();
            }

            void OnMenuButtonClicked()
            {
                if (!_didSetupView)
                    return;

                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("Apply to Addressables"), false, () =>
                {
                    var projectSettings = SmartAddresserProjectSettings.instance;
                    var primaryData = projectSettings.PrimaryData;

                    if (primaryData == null)
                    {
                        Apply();
                        return;
                    }

                    if (primaryData == _editingData.Value)
                    {
                        Apply();
                        return;
                    }

                    // If the primary data is not the same as the editing data, ask the user to confirm.
                    // If the user confirms, remove the primary data and apply the editing data.
                    var dialogMessage =
                        $"The {nameof(projectSettings.PrimaryData)} of the Project Settings is not the same as the data you are applying. Do you want to remove the {nameof(projectSettings.PrimaryData)} from Project Settings and apply the editing data?";
                    if (EditorUtility.DisplayDialog("Confirm", dialogMessage, "Remove & Apply", "Cancel"))
                    {
                        projectSettings.PrimaryData = null;
                        Apply();
                    }

                    void Apply()
                    {
                        // Apply the layout rules to the addressable asset system.
                        var layoutRule = _editingData.Value.LayoutRule;
                        var versionExpressionParser = new VersionExpressionParserRepository().Load();
                        var assetDatabaseAdapter = new AssetDatabaseAdapter();
                        var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
                        var addressableSettingsAdapter = new AddressableAssetSettingsAdapter(addressableSettings);
                        var applyService = new ApplyLayoutRuleService(layoutRule, versionExpressionParser,
                            addressableSettingsAdapter, assetDatabaseAdapter);
                        applyService.UpdateAllEntries();
                    }
                });

                menu.AddItem(new GUIContent("Open Layout Viewer"), false, LayoutViewerWindow.Open);

                menu.ShowAsContext();
            }

            void OnCreateButtonClicked()
            {
                var assetPath = EditorUtility.SaveFilePanelInProject($"Create {nameof(LayoutRuleData)}",
                    $"{nameof(LayoutRuleData)}", "asset", "", "Assets");
                if (string.IsNullOrEmpty(assetPath))
                    return;

                var asset = ScriptableObject.CreateInstance<LayoutRuleData>();
                AssetDatabase.CreateAsset(asset, assetPath);
                SetupActiveView(asset);
            }

            #endregion
        }

        private void CleanupViewEventHandlers()
        {
            _viewEventDisposables.Clear();
        }
    }
}
