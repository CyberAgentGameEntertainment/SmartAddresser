using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Layouts;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer
{
    /// <summary>
    ///     Presenter for <see cref="LayoutViewerView" />.
    /// </summary>
    internal sealed class LayoutViewerPresenter : IDisposable
    {
        private readonly BuildLayoutService _buildLayoutService;
        private readonly ObservableProperty<LayoutRuleData> _editingData = new ObservableProperty<LayoutRuleData>();
        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();
        private readonly LayoutViewerView _view;
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();
        private ILayoutRuleDataRepository _dataRepository;

        private bool _didSetupView;
        private Layout _layout;

        public LayoutViewerPresenter(BuildLayoutService buildLayoutService, LayoutViewerView view)
        {
            _buildLayoutService = buildLayoutService;
            _view = view;

            SetupViewEventHandlers();
        }

        public IReadOnlyObservableProperty<LayoutRuleData> EditingData => _editingData;

        public void Dispose()
        {
            CleanupView();
            CleanupViewEventHandlers();

            _editingData.Dispose();
        }

        private void AddGroupView(Group group, bool reload = true)
        {
            _view.TreeView.AddGroup(group);
            if (reload)
                _view.TreeView.Reload();
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
                _editingData.Value = null;
                return;
            }

            if (data == _editingData.Value)
                return;

            _editingData.Value = data;

            var projectSettings = SmartAddresserProjectSettings.instance;
            var validationSettings = projectSettings.ValidationSettings;
            var layout = _buildLayoutService.Execute(true, _editingData.Value.LayoutRule);
            layout.Validate(false, validationSettings.DuplicateAddresses, validationSettings.DuplicateAssetPaths,
                validationSettings.EntryHasMultipleVersions);
            _layout = layout;
            foreach (var group in layout.Groups)
                AddGroupView(group);
            _view.TreeView.Reload();
            _view.ActiveMode.Value = LayoutViewerView.Mode.Viewer;
            _view.ActiveAssetName.Value = data.name;

            _didSetupView = true;
        }

        public void CleanupView()
        {
            _setupViewDisposables.Clear();
            CleanupActiveView();
            _dataRepository = null;
        }

        private void CleanupActiveView()
        {
            _view.TreeView.ClearItems();
            _view.TreeView.Reload();
            _layout = null;
            _editingData.Value = null;
            _view.ActiveMode.Value = LayoutViewerView.Mode.Empty;

            _didSetupView = false;
        }

        private void SetupViewEventHandlers()
        {
            _view.BeforeLayoutAsObservable
                .Subscribe(_ => OnBeforeLayout())
                .DisposeWith(_viewEventDisposables);

            _view.AssetSelectButtonClickedAsObservable
                .Subscribe(_ => OnAssetSelectButtonClicked())
                .DisposeWith(_viewEventDisposables);

            _view.RefreshButtonClickedSubject
                .Subscribe(_ => OnRefreshButtonClicked())
                .DisposeWith(_viewEventDisposables);

            _view.TreeView.OnSelectionChanged += OnTreeViewSelectionChanged;

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
            }

            void OnAssetSelectButtonClicked()
            {
                if (!_didSetupView)
                    return;

                var menu = new GenericMenu();

                var sourceAssets = _dataRepository.LoadAll().ToArray();
                var sourceAssetNames = sourceAssets.Select(y => y.name).ToArray();
                var activeSourceAssetIndex = Array.IndexOf(sourceAssets, _editingData.Value);
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

            void OnRefreshButtonClicked()
            {
                var projectSettings = SmartAddresserProjectSettings.instance;
                var validationSettings = projectSettings.ValidationSettings;
                var layout = _buildLayoutService.Execute(true, _editingData.Value.LayoutRule);
                layout.Validate(false, validationSettings.DuplicateAddresses, validationSettings.DuplicateAssetPaths,
                    validationSettings.EntryHasMultipleVersions);
                _layout = layout;

                _view.TreeView.ClearItems();
                foreach (var group in layout.Groups)
                    AddGroupView(group);
                _view.TreeView.Reload();
            }

            #endregion
        }

        private void CleanupViewEventHandlers()
        {
            _view.TreeView.OnSelectionChanged -= OnTreeViewSelectionChanged;

            _viewEventDisposables.Clear();
        }

        private void OnTreeViewSelectionChanged(IList<int> ids)
        {
            Assert.IsNotNull(_layout);

            if (ids == null || ids.Count == 0)
            {
                _view.Message = string.Empty;
                return;
            }

            var id = ids[0];
            var treeViewItem = _view.TreeView.GetItem(id);
            if (treeViewItem is LayoutViewerTreeView.GroupItem groupItem)
            {
                _view.Message = string.Empty;
                var asset = groupItem.Group.AddressableGroup;
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
                return;
            }

            if (treeViewItem is LayoutViewerTreeView.EntryItem entryItem)
            {
                _view.Message = entryItem.Entry.Messages;
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(entryItem.Entry.AssetPath);
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
            }
        }
    }
}
