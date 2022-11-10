using System;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="LayoutRuleEditorView" />.
    /// </summary>
    internal sealed class LayoutRuleEditorPresenter : IDisposable
    {
        private readonly ObservableProperty<LayoutRuleData> _activeData = new ObservableProperty<LayoutRuleData>();
        private readonly IAddressableAssetSettingsRepository _addressableAssetSettingsRepository;
        private readonly AddressRuleEditorPresenter _addressRuleEditorPresenter;
        private readonly IAssetSaveService _assetSaveService;
        private readonly LabelRuleEditorPresenter _labelRuleEditorPresenter;
        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();
        private readonly VersionRuleEditorPresenter _versionRuleEditorPresenter;
        private readonly LayoutRuleEditorView _view;
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();
        private AddressableAssetSettings _addressableAssetSettings;

        private ILayoutRuleDataRepository _dataRepository;
        private bool _didSetupView;

        public LayoutRuleEditorPresenter(LayoutRuleEditorView view, AutoIncrementHistory history,
            IAssetSaveService assetSaveService, IAddressableAssetSettingsRepository addressableAssetSettingsRepository)
        {
            _view = view;
            _addressRuleEditorPresenter =
                new AddressRuleEditorPresenter(view.AddressRuleEditorView, history, assetSaveService);
            _labelRuleEditorPresenter =
                new LabelRuleEditorPresenter(view.LabelRuleEditorView, history, assetSaveService);
            _versionRuleEditorPresenter =
                new VersionRuleEditorPresenter(view.VersionRuleEditorView, history, assetSaveService);
            _assetSaveService = assetSaveService;
            _addressableAssetSettingsRepository = addressableAssetSettingsRepository;

            SetupViewEventHandlers();
        }

        public IReadOnlyObservableProperty<LayoutRuleData> ActiveData => _activeData;

        public void Dispose()
        {
            _addressRuleEditorPresenter.Dispose();
            _labelRuleEditorPresenter.Dispose();
            _versionRuleEditorPresenter.Dispose();
            CleanupView();
            CleanupViewEventHandlers();
            _activeData.Dispose();
        }

        public void SetupView(ILayoutRuleDataRepository dataRepository)
        {
            _setupViewDisposables.Clear();
            _dataRepository = dataRepository;
            dataRepository.ActiveData
                .Subscribe(SetupActiveView)
                .DisposeWith(_setupViewDisposables);
        }

        private void SetupActiveView(LayoutRuleData data)
        {
            CleanupActiveView();

            if (data == null)
            {
                _view.ActiveMode.Value = LayoutRuleEditorView.Mode.Create;
                _activeData.Value = null;
                _assetSaveService.SetAsset(data);
                return;
            }

            if (data == _activeData.Value)
                return;

            var addressableAssetSettings = _addressableAssetSettingsRepository.Get(data);
            _addressableAssetSettings = addressableAssetSettings;
            if (addressableAssetSettings == null)
            {
                _view.ActiveMode.Value = LayoutRuleEditorView.Mode.NoAddressableSettings;
                _activeData.Value = null;
                _assetSaveService.SetAsset(data);
                return;
            }

            _activeData.Value = data;
            _assetSaveService.SetAsset(data);

            data.LayoutRule.SyncAddressRulesWithAddressableAssetGroups(addressableAssetSettings.groups);
            _assetSaveService.MarkAsDirty();

            _addressRuleEditorPresenter.SetupView(data.LayoutRule.AddressRules);
            _labelRuleEditorPresenter.SetupView(data.LayoutRule.LabelRules);
            _versionRuleEditorPresenter.SetupView(data.LayoutRule.VersionRules);
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
                SetupActiveView(_activeData.Value);
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
            if (_addressableAssetSettings != null)
                _addressableAssetSettings.OnModification -= OnAddressableAssetSettingsModified;
            _addressableAssetSettings = null;
            _view.ActiveMode.Value = LayoutRuleEditorView.Mode.Create;
            _activeData.Value = null;
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
                if (_activeData.Value == null)
                {
                    var data = _dataRepository.LoadAll().FirstOrDefault();
                    if (data == null)
                    {
                        CleanupActiveView();
                    }
                    else
                    {
                        SetupActiveView(data);
                        _dataRepository.SetActiveData(data);
                    }
                }

                var addressableAssetSettings = _addressableAssetSettingsRepository.Get(_activeData.Value);
                // If the AddressableAssetSettings asset was deleted, reload.
                if (_view.ActiveMode.Value != LayoutRuleEditorView.Mode.NoAddressableSettings
                    && addressableAssetSettings == null)
                    SetupActiveView(_activeData.Value);

                // If the AddressableAssetSettings asset was created, reload.
                if (_view.ActiveMode.Value == LayoutRuleEditorView.Mode.NoAddressableSettings
                    && addressableAssetSettings != null)
                    SetupActiveView(_activeData.Value);
            }

            void OnAssetSelectButtonClicked()
            {
                if (!_didSetupView)
                    return;

                var menu = new GenericMenu();

                var sourceAssets = _dataRepository.LoadAll().ToList();
                var sourceAssetNames = sourceAssets.Select(y => y.name).ToArray();
                var activeSourceAssetIndex = sourceAssets.IndexOf(_activeData.Value);
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
                        _dataRepository.SetActiveData(asset);
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
                    // TODO: ルールを適用する仕組みを実装後、適用処理を書く
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
