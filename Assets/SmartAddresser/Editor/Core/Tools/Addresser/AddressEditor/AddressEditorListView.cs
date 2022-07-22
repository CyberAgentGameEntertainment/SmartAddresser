using SmartAddresser.Editor.Foundation.EasyTreeView;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.AddressEditor
{
    /// <summary>
    ///     View for the left side of the address editor.
    /// </summary>
    internal sealed class AddressEditorListView
    {
        private readonly TreeViewSearchField _searchField;

        public AddressEditorListView(AddressEditorTreeViewState treeViewState)
        {
            TreeView = new AddressRuleEditorTreeView(treeViewState);
            _searchField = new TreeViewSearchField(TreeView);
        }

        public AddressRuleEditorTreeView TreeView { get; }

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
