using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.EasyTreeView
{
    /// <summary>
    ///     The search field for the Easy Tree View.
    /// </summary>
    public class TreeViewSearchField
    {
        private readonly SearchField _searchField;
        private readonly Dictionary<int, string> _targetColumns = new Dictionary<int, string>();
        private readonly TreeViewBase _treeView;
        private int _selectedColumnIndex;

        /// <summary>
        ///     Initialize.
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="targetColumnIndices"></param>
        public TreeViewSearchField(TreeViewBase treeView, params int[] targetColumnIndices)
        {
            _searchField = new SearchField();
            var columns = treeView.multiColumnHeader?.state.columns;
            if (columns != null)
                for (var i = 0; i < columns.Length; i++)
                {
                    var column = columns[i];
                    if (targetColumnIndices.Length == 0 || targetColumnIndices.Contains(i))
                        _targetColumns.Add(i, column.headerContent.text);
                }

            _searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
            _treeView = treeView;
        }

        public string SearchString
        {
            get => _treeView.searchString;
            set => _treeView.searchString = value;
        }

        /// <summary>
        ///     <para>This is the controlID used for the text field to obtain keyboard focus.</para>
        /// </summary>
        public int SearchFieldControlID
        {
            get => _searchField.searchFieldControlID;
            set => _searchField.searchFieldControlID = value;
        }

        /// <summary>
        ///     <para>
        ///         Changes the keyboard focus to the search field when the user presses ‘Ctrl/Cmd + F’ when set to true. It is
        ///         true by default.
        ///     </para>
        /// </summary>
        public bool autoSetFocusOnFindCommand
        {
            get => _searchField.autoSetFocusOnFindCommand;
            set => _searchField.autoSetFocusOnFindCommand = value;
        }

        /// <summary>
        ///     <para>The event that is called when a down/up key is pressed.</para>
        /// </summary>
        public event SearchField.SearchFieldCallback downOrUpArrowKeyPressed
        {
            add => _searchField.downOrUpArrowKeyPressed += value;
            remove => _searchField.downOrUpArrowKeyPressed -= value;
        }

        /// <summary>
        ///     <para>This function changes keyboard focus to the search field so a user can start typing.</para>
        /// </summary>
        public void SetFocus()
        {
            _searchField.SetFocus();
        }

        /// <summary>
        ///     <para>This function returns true if the search field has keyboard focus.</para>
        /// </summary>
        public bool HasFocus()
        {
            return _searchField.HasFocus();
        }

        /// <summary>
        ///     <para>This function displays the search field with a toolbar style in the given Rect.</para>
        /// </summary>
        public string OnToolbarGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, string.Empty, EditorStyles.toolbarButton);
            rect.xMin += 10;
            var searchFieldRect = rect;
            searchFieldRect.y += 2;
            if (_targetColumns.Count >= 2)
            {
                var popupRect = rect;
                popupRect.y += 2;
                popupRect.width = Mathf.Min(popupRect.width, 100);
                searchFieldRect.x += popupRect.width + 9;
                searchFieldRect.width -= popupRect.width + 9;
                var currentColumnName = _targetColumns[_selectedColumnIndex];
                var buttonStyle = new GUIStyle(EditorStyles.popup);
                buttonStyle.fixedHeight = 16;
                if (GUI.Button(popupRect, $"Search : {currentColumnName}", buttonStyle))
                {
                    var menu = new GenericMenu();
                    foreach (var targetColumn in _targetColumns)
                    {
                        var columnName = targetColumn.Value;

                        // If the columnName contains a slash, replace it with the Unicode slash \u2215.
                        // Otherwise, the slash will be recognized as a menu hierarchy separator.
                        // https://answers.unity.com/questions/398495/can-genericmenu-item-content-display-.html
                        columnName = columnName.Replace("/", "\u2215");

                        var index = targetColumn.Key;
                        menu.AddItem(new GUIContent(columnName), _selectedColumnIndex == index, () =>
                        {
                            _selectedColumnIndex = index;
                            _treeView.SearchColumnIndex = _selectedColumnIndex;
                        });
                    }

                    menu.ShowAsContext();
                }
            }

            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var searchString = _searchField.OnToolbarGUI(searchFieldRect, _treeView.searchString);
                if (ccs.changed) _treeView.searchString = searchString;
                return searchString;
            }
        }

        /// <summary>
        ///     <para>This function displays the search field with a toolbar style.</para>
        /// </summary>
        public string OnToolbarGUI()
        {
            var maxWidth = _targetColumns.Count >= 2 ? 300 : 200;
            var rect = GUILayoutUtility.GetRect(100, maxWidth, EditorStyles.toolbar.fixedHeight,
                EditorStyles.toolbar.fixedHeight);
            return OnToolbarGUI(rect);
        }
    }
}
