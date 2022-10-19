using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.VersionRuleEditor
{
    internal sealed class VersionRuleListViewDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Version Rule List View";

        [SerializeField] private VersionRuleListTreeView.State _treeViewState;
        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();
        private readonly ObservableList<VersionRule> _rules = new ObservableList<VersionRule>();
        private VersionRuleListPresenter _presenter;
        private VersionRuleListView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);

            if (_treeViewState == null)
                _treeViewState = new VersionRuleListTreeView.State();
            _view = new VersionRuleListView(_treeViewState);
            _presenter = new VersionRuleListPresenter(_view, _history, new FakeAssetSaveService());
            _presenter.SetupView(_rules);
        }

        private void OnDisable()
        {
            _presenter?.Dispose();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Add VersionRule"))
                {
                    var versionRule = new VersionRule();
                    versionRule.Name.Value = $"Test {_rules.Count}";
                    _rules.Add(versionRule);
                }

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

                if (GUILayout.Button("Add VersionProvider"))
                {
                    var randomIndex = Random.Range(0, _rules.Count);
                    var randomRule = _rules[randomIndex];
                    randomRule.VersionProvider.Value = new AssetPathBasedVersionProvider();
                    randomRule.RefreshVersionProviderDescription();
                }
            }

            _view.DoLayout();
        }

        [MenuItem("Window/Smart Addresser/Development/Addresser/Version Rule Editor/Version Rule List View")]
        public static void Open()
        {
            GetWindow<VersionRuleListViewDevelopmentWindow>(WindowName);
        }
    }
}
