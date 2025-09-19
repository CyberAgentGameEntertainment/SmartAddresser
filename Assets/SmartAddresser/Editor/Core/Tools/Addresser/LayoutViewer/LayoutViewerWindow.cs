using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer
{
    internal sealed class LayoutViewerWindow : EditorWindow
    {
        private const string WindowName = "Layout Viewer";

        [SerializeField] private LayoutViewerTreeView.State _treeViewState = new LayoutViewerTreeView.State();
        [SerializeField] private string _searchFieldText;
        [SerializeField] private EditorGUILayoutSplitViewState _splitViewState;
        private readonly CompositeDisposable _setupDisposables = new CompositeDisposable();

        private LayoutViewerPresenter _presenter;
        private LayoutViewerView _view;

        private AddressableAssetSettings _addressableAssetSettings;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);
            Setup();
            
            _addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
            if (_addressableAssetSettings)
            {
                _addressableAssetSettings.OnModification += OnAddressableAssetSettingsModified;
            }
        }

        private void OnDisable()
        {
            if (_addressableAssetSettings)
            {
                _addressableAssetSettings.OnModification -= OnAddressableAssetSettingsModified;
                _addressableAssetSettings = null;
            }
            
            _presenter?.Dispose();
            _view?.Dispose();
        }

        private void OnDestroy()
        {
            _setupDisposables.Dispose();
        }

        private void OnGUI()
        {
            _view.DoLayout();
        }

        private void Setup(LayoutRuleData initialData = null)
        {
            _setupDisposables.Clear();
            _presenter?.Dispose();
            _view?.Dispose();

            if (_splitViewState == null)
                _splitViewState = new EditorGUILayoutSplitViewState(LayoutDirection.Vertical, 0.75f);

            var buildLayoutService = new BuildLayoutService(new AssetDatabaseAdapter());
            _view = new LayoutViewerView(_treeViewState, _splitViewState, _searchFieldText, Repaint);
            _view.SearchText
                .Subscribe(x => _searchFieldText = x)
                .DisposeWith(_setupDisposables);
            _presenter = new LayoutViewerPresenter(buildLayoutService, _view);
            _presenter.SetupView(new LayoutRuleDataRepository());
        }

        [MenuItem("Window/Smart Addresser/Layout Viewer")]
        public static void Open()
        {
            GetWindow<LayoutViewerWindow>(WindowName);
        }

        private void OnAddressableAssetSettingsModified(
            AddressableAssetSettings settings,
            AddressableAssetSettings.ModificationEvent e,
            object obj)
        {
            // AssetGroupの並び順反映
            if (e == AddressableAssetSettings.ModificationEvent.GroupMoved)
            {
                _presenter.ApplyGroupsToTreeView();
            }
            
            // コールバックを末尾に登録しなおす
            // AssetGroupの並び順はOnModificationコールバック内でシリアライズされるので、それより後に更新しないと反映できないため
            if (_addressableAssetSettings)
            {
                _addressableAssetSettings.OnModification -= OnAddressableAssetSettingsModified;
                _addressableAssetSettings.OnModification += OnAddressableAssetSettingsModified;
            }
        }
    }
}
