using System;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    internal sealed class AddressRuleEditorInspectorView : IDisposable
    {
        public enum Tab
        {
            AssetGroups,
            AddressRule
        }

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ObservableProperty<Tab> _tabType = new ObservableProperty<Tab>(Tab.AssetGroups);

        public IObservableProperty<Tab> TabType => _tabType;
        public AssetGroupCollectionPanelView GroupCollectionView { get; } = new AssetGroupCollectionPanelView();
        public AddressProviderPanelView AddressProviderPanelView { get; } = new AddressProviderPanelView();

        public bool Enabled { get; set; }

        public void Dispose()
        {
            _disposables.Dispose();
            _tabType.Dispose();
            AddressProviderPanelView.Dispose();
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
                    var isActive = _tabType.Value == Tab.AddressRule;
                    isActive = GUILayout.Toggle(isActive, "Address Provider", EditorStyles.toolbarButton,
                        GUILayout.Width(110));
                    if (ccs.changed && isActive)
                        _tabType.Value = Tab.AddressRule;
                }

                GUILayout.FlexibleSpace();
            }

            switch (_tabType.Value)
            {
                case Tab.AssetGroups:
                    GroupCollectionView.DoLayout();
                    break;
                case Tab.AddressRule:
                    AddressProviderPanelView.DoLayout();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GUI.enabled = enabled;
        }
    }
}
