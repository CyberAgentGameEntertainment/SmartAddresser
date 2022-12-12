using System;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.SettingsEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor
{
    /// <summary>
    ///     View for the layout rule editor.
    /// </summary>
    internal sealed class LayoutRuleEditorView : IDisposable
    {
        public enum Mode
        {
            Create,
            Edit,
            NoAddressableSettings
        }

        public enum Tab
        {
            AddressRules,
            LabelRules,
            VersionRules,
            Settings
        }

        private readonly ObservableProperty<string> _activeAssetName = new ObservableProperty<string>();

        private readonly ObservableProperty<Mode> _activeMode = new ObservableProperty<Mode>();
        private readonly ObservableProperty<Tab> _activeTab = new ObservableProperty<Tab>();
        private readonly Subject<Empty> _assetSelectButtonClickedSubject = new Subject<Empty>();

        private readonly Subject<Empty> _beforeLayoutSubject = new Subject<Empty>();
        private readonly Subject<Empty> _createButtonClickedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _menuButtonClickedSubject = new Subject<Empty>();

        public LayoutRuleEditorView(AddressRuleListTreeView.State addressTreeViewState,
            LabelRuleListTreeView.State labelTreeViewState, VersionRuleListTreeView.State versionTreeViewState,
            EditorGUILayoutSplitViewState splitViewState, Action repaintParentWindow)
        {
            AddressRuleEditorView =
                new AddressRuleEditorView(addressTreeViewState, splitViewState, repaintParentWindow);
            LabelRuleEditorView = new LabelRuleEditorView(labelTreeViewState, splitViewState, repaintParentWindow);
            VersionRuleEditorView =
                new VersionRuleEditorView(versionTreeViewState, splitViewState, repaintParentWindow);
            SettingsEditorView = new SettingsEditorView();
        }

        private string CreateViewMessage { get; } =
            $"{nameof(LayoutRuleData)} cannot be found. Click the following button to create it.";

        private string NoAddressableDataViewMessage { get; } =
            $"{nameof(AddressableAssetSettings)} cannot be found. Please create it before using Smart Addresser.";

        public AddressRuleEditorView AddressRuleEditorView { get; }
        public LabelRuleEditorView LabelRuleEditorView { get; }
        public VersionRuleEditorView VersionRuleEditorView { get; }
        public SettingsEditorView SettingsEditorView { get; }
        public IObservableProperty<Tab> ActiveTab => _activeTab;
        public IObservableProperty<Mode> ActiveMode => _activeMode;
        public IObservableProperty<string> ActiveAssetName => _activeAssetName;
        public IObservable<Empty> MenuButtonClickedAsObservable => _menuButtonClickedSubject;
        public IObservable<Empty> BeforeLayoutAsObservable => _beforeLayoutSubject;
        public IObservable<Empty> CreateButtonClickedAsObservable => _createButtonClickedSubject;
        public IObservable<Empty> AssetSelectButtonClickedAsObservable => _assetSelectButtonClickedSubject;

        public void Dispose()
        {
            AddressRuleEditorView.Dispose();
            LabelRuleEditorView.Dispose();
            VersionRuleEditorView.Dispose();
            SettingsEditorView.Dispose();
            _activeTab.Dispose();
            _activeMode.Dispose();
            _menuButtonClickedSubject.Dispose();
            _beforeLayoutSubject.Dispose();
            _createButtonClickedSubject.Dispose();
            _assetSelectButtonClickedSubject.Dispose();
            _activeAssetName.Dispose();
        }

        public void DoLayout()
        {
            _beforeLayoutSubject.OnNext(Empty.Default);

            switch (ActiveMode.Value)
            {
                case Mode.Create:
                    DrawCreateView();
                    break;
                case Mode.Edit:
                    DrawEditView();
                    break;
                case Mode.NoAddressableSettings:
                    DrawNoAddressableSettingsView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawEditView()
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

                if (GUILayout.Button(ActiveAssetName.Value, EditorStyles.toolbarDropDown, GUILayout.Width(120)))
                    _assetSelectButtonClickedSubject.OnNext(Empty.Default);

                // Menu Button
                var menuButtonRect = GUILayoutUtility.GetRect(24, 20);
                var menuButtonImageRect = menuButtonRect;
                menuButtonImageRect.xMin += 2;
                menuButtonImageRect.xMax -= 2;
                var menuIconTexture = EditorGUIUtility.IconContent(EditorGUIUtil.MenuIconName).image;
                GUI.DrawTexture(menuButtonImageRect, menuIconTexture, ScaleMode.StretchToFill);
                if (GUI.Button(menuButtonRect, "", GUIStyle.none))
                    _menuButtonClickedSubject.OnNext(Empty.Default);
            }

            switch (ActiveTab.Value)
            {
                case Tab.AddressRules:
                    AddressRuleEditorView.DoLayout();
                    break;
                case Tab.LabelRules:
                    LabelRuleEditorView.DoLayout();
                    break;
                case Tab.VersionRules:
                    VersionRuleEditorView.DoLayout();
                    break;
                case Tab.Settings:
                    SettingsEditorView.DoLayout();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawCreateView()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(CreateViewMessage);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button($"Create {nameof(LayoutRuleData)}", GUILayout.Width(200)))
                _createButtonClickedSubject.OnNext(Empty.Default);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private void DrawNoAddressableSettingsView()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(NoAddressableDataViewMessage);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private static string GetTabName(Tab tab)
        {
            return tab switch
            {
                Tab.AddressRules => "Address Rules",
                Tab.LabelRules => "Label Rules",
                Tab.VersionRules => "Version Rules",
                Tab.Settings => "Settings",
                _ => throw new ArgumentOutOfRangeException(nameof(tab), tab, null)
            };
        }
    }
}
