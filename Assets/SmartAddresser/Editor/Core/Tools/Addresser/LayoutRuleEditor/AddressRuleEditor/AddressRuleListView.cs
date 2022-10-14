using SmartAddresser.Editor.Foundation.EasyTreeView;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     View for the left side of the address rule editor.
    /// </summary>
    internal sealed class AddressRuleListView
    {
        private readonly TreeViewSearchField _searchField;

        public AddressRuleListView(AddressRuleListTreeView.State treeViewState)
        {
            TreeView = new AddressRuleListTreeView(treeViewState);
            _searchField = new TreeViewSearchField(TreeView);
        }

        public AddressRuleListTreeView TreeView { get; }

        public void DoLayout()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // Search Field
                _searchField.OnToolbarGUI();

                GUILayout.FlexibleSpace();
            }

            // Tree View
            var treeViewRect =
                GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            TreeView.OnGUI(treeViewRect);
        }
    }
}
