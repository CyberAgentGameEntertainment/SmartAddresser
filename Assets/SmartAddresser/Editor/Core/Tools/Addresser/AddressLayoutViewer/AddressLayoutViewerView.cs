using SmartAddresser.Editor.Foundation.EasyTreeView;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.AddressLayoutViewer
{
    /// <summary>
    ///     View for the address viewer.
    /// </summary>
    internal sealed class AddressLayoutViewerView
    {
        private readonly TreeViewSearchField _searchField;

        public AddressLayoutViewerView(AddressLayoutViewerTreeView.State treeViewState)
        {
            TreeView = new AddressLayoutViewerTreeView(treeViewState);
            _searchField = new TreeViewSearchField(TreeView);
        }

        public AddressLayoutViewerTreeView TreeView { get; }

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
