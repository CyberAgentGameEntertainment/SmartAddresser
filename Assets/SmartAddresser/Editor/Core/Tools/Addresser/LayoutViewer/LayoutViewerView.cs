using System;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation.EasyTreeView;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer
{
    /// <summary>
    ///     View for the address viewer.
    /// </summary>
    internal sealed class LayoutViewerView : IDisposable
    {
        public enum Mode
        {
            Empty,
            Viewer
        }

        private readonly ObservableProperty<string> _activeAssetName = new ObservableProperty<string>();

        private readonly ObservableProperty<Mode> _activeMode = new ObservableProperty<Mode>();
        private readonly Subject<Empty> _assetSelectButtonClickedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _beforeLayoutSubject = new Subject<Empty>();
        private readonly Subject<Empty> _refreshButtonClickedSubject = new Subject<Empty>();
        private readonly Action _repaintParentWindow;
        private readonly TreeViewSearchField _searchField;
        private readonly ObservableProperty<string> _searchText = new ObservableProperty<string>();
        private readonly EditorGUILayoutSplitView _splitView;

        public LayoutViewerView(LayoutViewerTreeView.State treeViewState, EditorGUILayoutSplitViewState splitViewState,
            string searchFieldText, Action repaintParentWindow)
        {
            TreeView = new LayoutViewerTreeView(treeViewState);
            _splitView = new EditorGUILayoutSplitView(splitViewState);
            _searchField = new TreeViewSearchField(TreeView);
            _searchText.SetValueAndNotNotify(searchFieldText);
            _searchField.SearchString = searchFieldText;
            _repaintParentWindow = repaintParentWindow;
        }

        public IObservableProperty<string> ActiveAssetName => _activeAssetName;
        public IObservable<Empty> AssetSelectButtonClickedAsObservable => _assetSelectButtonClickedSubject;

        public string Message { get; set; }

        public IObservable<Empty> RefreshButtonClickedSubject => _refreshButtonClickedSubject;
        public IReadOnlyObservableProperty<string> SearchText => _searchText;
        public IObservableProperty<Mode> ActiveMode => _activeMode;
        public IObservable<Empty> BeforeLayoutAsObservable => _beforeLayoutSubject;

        public LayoutViewerTreeView TreeView { get; }

        private string EmptyViewMessage { get; } =
            $"{nameof(LayoutRuleData)} cannot be found. Please create a new one and select it from the drop-down in the upper right corner. ";

        public void Dispose()
        {
            _activeMode.Dispose();
            _searchText.Dispose();
            _beforeLayoutSubject.Dispose();
            _refreshButtonClickedSubject.Dispose();
            _activeAssetName.Dispose();
            _assetSelectButtonClickedSubject.Dispose();
        }

        public void DoLayout()
        {
            _beforeLayoutSubject.OnNext(Empty.Default);

            switch (_activeMode.Value)
            {
                case Mode.Empty:
                    DrawEmptyView();
                    break;
                case Mode.Viewer:
                    DrawViewerView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawViewerView()
        {
            _splitView.Begin();

            DrawTopView();

            if (_splitView.Split())
                _repaintParentWindow.Invoke();

            DrawBottomView();

            _splitView.End();
        }

        private void DrawTopView()
        {
            // Toolbar
            DrawToolbar();

            // Tree View
            var treeViewRect =
                GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            TreeView.OnGUI(treeViewRect);
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // Refresh Button
                var refreshButtonRect = GUILayoutUtility.GetRect(24, 20, GUILayout.ExpandWidth(false));
                var refreshButtonImageRect = refreshButtonRect;
                refreshButtonImageRect.xMin += 4;
                refreshButtonImageRect.xMax -= 4;
                refreshButtonImageRect.yMin += 2;
                refreshButtonImageRect.yMax -= 2;
                var refreshIconTexture = EditorGUIUtility.IconContent(EditorGUIUtil.RefreshIconName).image;
                GUI.DrawTexture(refreshButtonImageRect, refreshIconTexture, ScaleMode.StretchToFill);
                if (GUI.Button(refreshButtonRect, "", GUIStyle.none))
                    _refreshButtonClickedSubject.OnNext(Empty.Default);

                // Search Field
                _searchText.Value = _searchField.OnToolbarGUI();

                // Asset Select Button
                if (GUILayout.Button(ActiveAssetName.Value, EditorStyles.toolbarDropDown, GUILayout.Width(120)))
                    _assetSelectButtonClickedSubject.OnNext(Empty.Default);
            }
        }

        private void DrawBottomView()
        {
            GUILayout.Label(Message);
        }

        private void DrawEmptyView()
        {
            DrawToolbar();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(EmptyViewMessage);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}
