using System;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    internal sealed class LabelRuleEditorInspectorView : IDisposable
    {
        public enum Tab
        {
            AssetGroups,
            LabelRule
        }

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ObservableProperty<Tab> _tabType = new ObservableProperty<Tab>(Tab.AssetGroups);

        public IObservableProperty<Tab> TabType => _tabType;
        public AssetGroupCollectionPanelView GroupCollectionView { get; } = new AssetGroupCollectionPanelView();
        public LabelProviderPanelView LabelProviderPanelView { get; } = new LabelProviderPanelView();

        public bool Enabled { get; set; }

        public void Dispose()
        {
            _disposables.Dispose();
            _tabType.Dispose();
            LabelProviderPanelView.Dispose();
            GroupCollectionView.Dispose();
        }

        public void DoLayout()
        {
            var enabled = GUI.enabled;
            GUI.enabled = GUI.enabled && Enabled;

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    var isActive = _tabType.Value == Tab.AssetGroups;
                    isActive = GUILayout.Toggle(isActive, "Asset Groups", EditorStyles.toolbarButton,
                        GUILayout.Width(110));
                    if (ccs.changed && isActive)
                        _tabType.Value = Tab.AssetGroups;
                }

                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    var isActive = _tabType.Value == Tab.LabelRule;
                    isActive = GUILayout.Toggle(isActive, "Label Provider", EditorStyles.toolbarButton,
                        GUILayout.Width(110));
                    if (ccs.changed && isActive)
                        _tabType.Value = Tab.LabelRule;
                }

                GUILayout.FlexibleSpace();
            }

            switch (_tabType.Value)
            {
                case Tab.AssetGroups:
                    GroupCollectionView.DoLayout();
                    break;
                case Tab.LabelRule:
                    LabelProviderPanelView.DoLayout();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GUI.enabled = enabled;
        }
    }
}
