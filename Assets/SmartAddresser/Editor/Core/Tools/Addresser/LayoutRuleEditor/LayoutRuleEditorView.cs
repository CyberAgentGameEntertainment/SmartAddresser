using System;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor
{
    /// <summary>
    ///     View for the layout rule editor.
    /// </summary>
    internal sealed class LayoutRuleEditorView : IDisposable
    {
        public enum Tab
        {
            AddressRule,
            LabelRule,
            VersionRule,
            Settings
        }

        private readonly Subject<Empty> _applyButtonClickedSubject = new Subject<Empty>();

        public LayoutRuleEditorView(AddressRuleListTreeView.State addressTreeViewState,
            LabelRuleListTreeView.State labelTreeViewState, VersionRuleListTreeView.State versionTreeViewState,
            EditorGUILayoutSplitViewState splitViewState, Action repaintParentWindow)
        {
            AddressRuleEditorView =
                new AddressRuleEditorView(addressTreeViewState, splitViewState, repaintParentWindow);
            LabelRuleEditorView = new LabelRuleEditorView(labelTreeViewState, splitViewState, repaintParentWindow);
            VersionRuleEditorView =
                new VersionRuleEditorView(versionTreeViewState, splitViewState, repaintParentWindow);
        }

        public AddressRuleEditorView AddressRuleEditorView { get; }
        public LabelRuleEditorView LabelRuleEditorView { get; }
        public VersionRuleEditorView VersionRuleEditorView { get; }

        public ObservableProperty<Tab> ActiveTab { get; } = new ObservableProperty<Tab>();

        public IObservable<Empty> ApplyButtonClickedAsObservable => _applyButtonClickedSubject;

        public void Dispose()
        {
            AddressRuleEditorView.Dispose();
            LabelRuleEditorView.Dispose();
            VersionRuleEditorView.Dispose();
            ActiveTab.Dispose();
            _applyButtonClickedSubject.Dispose();
        }

        public void DoLayout()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.ExpandWidth(true)))
            {
                foreach (Tab tab in Enum.GetValues(typeof(Tab)))
                {
                    var isActive = tab == ActiveTab.Value;
                    using var ccs = new EditorGUI.ChangeCheckScope();
                    var result = GUILayout.Toggle(isActive, GetTabName(tab), EditorStyles.toolbarButton);
                    if (ccs.changed && result)
                        ActiveTab.Value = tab;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Apply", EditorStyles.toolbarButton))
                    _applyButtonClickedSubject.OnNext(Empty.Default);
            }

            switch (ActiveTab.Value)
            {
                case Tab.AddressRule:
                    AddressRuleEditorView.DoLayout();
                    break;
                case Tab.LabelRule:
                    LabelRuleEditorView.DoLayout();
                    break;
                case Tab.VersionRule:
                    VersionRuleEditorView.DoLayout();
                    break;
                case Tab.Settings:
                    //TODO: SettingsタブのViewを実装後に対応
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetTabName(Tab tab)
        {
            return tab switch
            {
                Tab.AddressRule => "Address Rule",
                Tab.LabelRule => "Label Rule",
                Tab.VersionRule => "Version Rule",
                Tab.Settings => "Settings",
                _ => throw new ArgumentOutOfRangeException(nameof(tab), tab, null)
            };
        }
    }
}
