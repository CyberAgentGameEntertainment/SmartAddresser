using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser.LabelRuleEditor
{
    internal sealed class LabelRuleListViewDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Label Rule List View";

        [SerializeField] private LabelRuleListTreeView.State _treeViewState;
        private readonly AutoIncrementHistory _history = new AutoIncrementHistory();
        private readonly ObservableList<LabelRule> _rules = new ObservableList<LabelRule>();
        private LabelRuleListPresenter _presenter;
        private LabelRuleListView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);

            if (_treeViewState == null)
                _treeViewState = new LabelRuleListTreeView.State();
            _view = new LabelRuleListView(_treeViewState);
            _presenter = new LabelRuleListPresenter(_view, _history, new FakeAssetSaveService());
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
                if (GUILayout.Button("Add LabelRule"))
                {
                    var labelRule = new LabelRule();
                    labelRule.Name.Value = $"Test {_rules.Count}";
                    _rules.Add(labelRule);
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

                if (GUILayout.Button("Add LabelProvider"))
                {
                    var randomIndex = Random.Range(0, _rules.Count);
                    var randomRule = _rules[randomIndex];
                    randomRule.LabelProvider.Value = new AssetPathBasedLabelProvider();
                    randomRule.RefreshLabelProviderDescription();
                }
            }

            _view.DoLayout();
        }

        [MenuItem("Window/Smart Addresser/Development/Addresser/Label Rule Editor/Label Rule List View")]
        public static void Open()
        {
            GetWindow<LabelRuleListViewDevelopmentWindow>(WindowName);
        }
    }
}
