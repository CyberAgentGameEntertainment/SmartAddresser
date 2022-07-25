using SmartAddresser.Editor.Foundation.EasyTreeView;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer
{
    /// <summary>
    ///     View for the address viewer.
    /// </summary>
    internal sealed class LayoutViewerView
    {
        private readonly TreeViewSearchField _searchField;

        public LayoutViewerView(LayoutViewerTreeView.State treeViewState)
        {
            TreeView = new LayoutViewerTreeView(treeViewState);
            _searchField = new TreeViewSearchField(TreeView);
        }

        public LayoutViewerTreeView TreeView { get; }

        public void DoLayout()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // Search Field
                _searchField.OnToolbarGUI();
            }

            // Tree View
            var treeViewRect =
                GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            TreeView.OnGUI(treeViewRect);
        }
    }
}
