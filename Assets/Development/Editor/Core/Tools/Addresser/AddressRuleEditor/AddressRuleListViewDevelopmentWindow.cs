using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.AddressRuleEditor
{
    internal sealed class AddressRuleListViewDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Address Rule List View";

        [SerializeField] private AddressRuleListTreeView.State _treeViewState;
        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();
        private readonly ObservableList<AddressRule> _rules = new ObservableList<AddressRule>();
        private AddressRuleListPresenter _presenter;
        private AddressRuleListView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);

            if (_treeViewState == null)
                _treeViewState = new AddressRuleListTreeView.State();
            _view = new AddressRuleListView(_treeViewState);
            _presenter = new AddressRuleListPresenter(_view, _history, new FakeAssetSaveService());
            _presenter.SetupView(_rules);

            // Use default settings for development.
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddGroups(settings);
            settings.OnModification += OnAddressableAssetSettingsModification;
        }

        private void OnDisable()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            settings.OnModification -= OnAddressableAssetSettingsModification;
            _presenter?.Dispose();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Add AssetFilter"))
                {
                    var randomIndex = Random.Range(0, _rules.Count);
                    var randomRule = _rules[randomIndex];
                    var assetGroup = new AssetGroup();
                    var extensionFilter = new ExtensionBasedAssetFilter();
                    extensionFilter.Extension.Value = "png";
                    assetGroup.Filters.Add(extensionFilter);
                    randomRule.AssetGroups.Add(assetGroup);
                    randomRule.RefreshAssetGroupDescription();
                }

                if (GUILayout.Button("Add AddressProvider"))
                {
                    var randomIndex = Random.Range(0, _rules.Count);
                    var randomRule = _rules[randomIndex];
                    randomRule.AddressProvider.Value = new AssetPathBasedAddressProvider();
                    randomRule.RefreshAddressProviderDescription();
                }
            }

            _view.DoLayout();
        }

        private void OnAddressableAssetSettingsModification(AddressableAssetSettings settings,
            AddressableAssetSettings.ModificationEvent modificationEvent, object obj)
        {
            // Add Group.
            if (modificationEvent == AddressableAssetSettings.ModificationEvent.GroupAdded)
            {
                var addressableGroup = (AddressableAssetGroup)obj;
                AddGroup(addressableGroup);
            }

            // Remove Group.
            else if (modificationEvent == AddressableAssetSettings.ModificationEvent.GroupRemoved)
            {
                // Clean up all rules that have the missing addressable group.
                if (obj == null)
                {
                    RemoveMissingGroupRules();
                    return;
                }

                if (obj is AddressableAssetGroup addressableGroup)
                    RemoveGroup(addressableGroup);
            }

            // Move Group.
            else if (modificationEvent == AddressableAssetSettings.ModificationEvent.GroupMoved)
            {
                ClearGroups();
                AddGroups(settings);
            }
        }

        private void AddGroups(AddressableAssetSettings settings)
        {
            foreach (var addressableGroup in settings.groups)
                AddGroup(addressableGroup);
        }

        private void AddGroup(AddressableAssetGroup addressableGroup)
        {
            var rule = new AddressRule(addressableGroup);
            _rules.Add(rule);
        }

        private void RemoveGroup(AddressableAssetGroup addressableGroup)
        {
            var rule = _rules.First(x => addressableGroup == x.AddressableGroup);
            _rules.Remove(rule);
        }

        private void RemoveMissingGroupRules()
        {
            var missingGroupRules = _rules.Where(x => x.AddressableGroup == null).ToArray();
            foreach (var missingGroupRule in missingGroupRules)
                _rules.Remove(missingGroupRule);
        }

        private void ClearGroups()
        {
            _rules.Clear();
        }

        [MenuItem("Window/Smart Addresser/Development/Addresser/Address Rule Editor/Address Rule List View")]
        public static void Open()
        {
            GetWindow<AddressRuleListViewDevelopmentWindow>(WindowName);
        }
    }
}
