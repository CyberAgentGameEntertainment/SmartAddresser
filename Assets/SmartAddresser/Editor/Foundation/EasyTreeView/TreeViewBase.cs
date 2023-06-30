// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.EasyTreeView
{
    /// <summary>
    ///     The base class of the Easy Tree View.
    /// </summary>
    public abstract class TreeViewBase : TreeView
    {
        private readonly Dictionary<int, TreeViewItem> _items = new Dictionary<int, TreeViewItem>();
        private MultiColumnHeaderState.Column[] _columnStates;
        private bool _isSortingNeeded;
        private int _searchColumnIndex;

        /// <summary>
        ///     Initialize.
        /// </summary>
        /// <param name="treeViewState"></param>
        protected TreeViewBase(TreeViewState treeViewState) : base(treeViewState)
        {
            var root = new TreeViewItem
            {
                id = -1,
                displayName = "Root",
                depth = -1
            };
            RootItem = root;
        }

        public TreeViewItem RootItem { get; }

        /// <summary>
        ///     The column index to be searched for.
        /// </summary>
        public int SearchColumnIndex
        {
            get => _searchColumnIndex;
            set
            {
                _searchColumnIndex = value;
                Reload();
            }
        }

        public Func<GenericMenu> RightClickMenuRequested { get; set; }

        /// <summary>
        ///     If you want to use multiple columns, override this property and specify the column information.
        /// </summary>
        protected MultiColumnHeaderState.Column[] ColumnStates
        {
            get => _columnStates;
            set
            {
                if (value == null || value.Length == 0)
                {
                    multiColumnHeader = null;
                }
                else
                {
                    multiColumnHeader = CreateMultiColumnHeader(new MultiColumnHeaderState(value));
                    multiColumnHeader.sortingChanged += OnSortingChanged;
                }

                _columnStates = value;
            }
        }

        protected virtual MultiColumnHeader CreateMultiColumnHeader(MultiColumnHeaderState headerState)
        {
            return new MultiColumnHeader(headerState);
        }

        /// <summary>
        ///     Callback for when the item is added.
        /// </summary>
        public event Action<TreeViewItem> OnItemAdded;

        /// <summary>
        ///     Callback for when the item is removed.
        /// </summary>
        public event Action<TreeViewItem> OnItemRemoved;

        /// <summary>
        ///     Callback for when all items are cleared.
        /// </summary>
        public event Action OnItemsCleared;

        /// <summary>
        ///     Callback for when the selected column is switched.
        /// </summary>
        public event Action<IList<int>> OnSelectionChanged;

        public event Action<int> ItemClicked;

        public event Action<int> ItemDoubleClicked;

        public bool HasItem(int id)
        {
            return _items.ContainsKey(id);
        }

        /// <summary>
        ///     Get an item.
        /// </summary>
        public TreeViewItem GetItem(int id)
        {
            return _items[id];
        }

        /// <summary>
        ///     Add an item and set the parent.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parentId"></param>
        /// <param name="invokeCallback"></param>
        /// <param name="index"></param>
        public void AddItemAndSetParent(TreeViewItem item, int parentId, int index = -1, bool invokeCallback = true)
        {
            var parent = parentId == -1 ? RootItem : _items[parentId];
            parent.AddChild(item);
            _items.Add(item.id, item);
            if (index != -1)
            {
                // Set item index.
                parent.children.RemoveAt(_items.Count - 1);
                parent.children.Insert(index, item);
            }

            if (invokeCallback) OnItemAdded?.Invoke(item);
        }

        /// <summary>
        ///     Remove an item.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="invokeCallback"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void RemoveItem(int id, bool invokeCallback = true)
        {
            if (id == -1 || !_items.ContainsKey(id)) throw new InvalidOperationException();

            var item = _items[id];
            var parent = rootItem;
            // If the item has a parent other than the root and the parent has not been removed, the parent is specified.
            if (item.parent.id != -1 && !_items.TryGetValue(item.parent.id, out parent))
            {
                _items.Remove(id);
                return;
            }

            parent.children.Remove(item);
            _items.Remove(id);
            if (invokeCallback) OnItemRemoved?.Invoke(item);
        }

        /// <summary>
        ///     Clear all items.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void ClearItems(bool invokeCallback = true)
        {
            rootItem.children?.Clear();
            _items.Clear();

            if (invokeCallback)
                OnItemsCleared?.Invoke();
        }

        /// <summary>
        ///     Draw cell.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="cellRect"></param>
        /// <param name="args"></param>
        protected virtual void CellGUI(int columnIndex, Rect cellRect, RowGUIArgs args)
        {
            base.RowGUI(args);
        }

        protected void DefaultRowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
        }

        /// <summary>
        ///     Order items.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="keyColumnIndex"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected abstract IOrderedEnumerable<TreeViewItem> OrderItems(IList<TreeViewItem> items, int keyColumnIndex,
            bool ascending);

        /// <summary>
        ///     Get the text to be used for the search.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        protected abstract string GetTextForSearch(TreeViewItem item, int columnIndex);

        protected override void RowGUI(RowGUIArgs args)
        {
            if (multiColumnHeader == null)
                CellGUI(0, args.rowRect, args);
            else
                for (var i = 0; i < args.GetNumVisibleColumns(); ++i)
                {
                    var cellRect = args.GetCellRect(i);
                    var columnIndex = args.GetColumn(i);
                    CellGUI(columnIndex, cellRect, args);
                }
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            if (multiColumnHeader != null)
            {
                rect.height -= multiColumnHeader.height;
                rect.y += multiColumnHeader.height;
            }

            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown &&
                Event.current.button == 1)
                RightClickMenuRequested?.Invoke().ShowAsContext();
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            OnSelectionChanged?.Invoke(selectedIds);
        }

        private bool DoesCellMatchSearch(TreeViewItem item, int columnIndex, string search)
        {
            var text = GetTextForSearch(item, columnIndex);
            if (string.IsNullOrEmpty(text))
                return false;
            return text.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected override TreeViewItem BuildRoot()
        {
            return RootItem;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = base.BuildRows(root);
            SortIfNeeded();
            SetupDepthsFromParentsAndChildren(root);
            return rows;
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            return DoesCellMatchSearch(item, SearchColumnIndex, search);
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            ItemClicked?.Invoke(id);
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            ItemDoubleClicked?.Invoke(id);
        }

        private void OnSortingChanged(MultiColumnHeader header)
        {
            _isSortingNeeded = true;
            SortIfNeeded();
            Reload();
        }

        private void SortIfNeeded()
        {
            if (!_isSortingNeeded || multiColumnHeader == null) return;

            var keyColumnIndex = 0;
            var ascending = true;
            if (multiColumnHeader.sortedColumnIndex >= 0)
            {
                keyColumnIndex = multiColumnHeader.sortedColumnIndex;
                ascending = multiColumnHeader.IsSortedAscending(keyColumnIndex);
            }

            SortHierarchical(rootItem.children, keyColumnIndex, ascending);

            _isSortingNeeded = false;
        }

        private void SortHierarchical(IList<TreeViewItem> children, int keyColumnIndex, bool ascending)
        {
            if (children == null) return;

            var depth = children[0].depth;
            var isSkipSortingTarget = GetSkipSortingDepths().Contains(depth);
            var orderedChildren = isSkipSortingTarget
                ? new List<TreeViewItem>(children)
                : OrderItems(children, keyColumnIndex, ascending).ToList();

            children.Clear();
            foreach (var orderedChild in orderedChildren) children.Add(orderedChild);

            foreach (var child in children)
                if (child != null)
                    SortHierarchical(child.children, keyColumnIndex, ascending);
        }

        protected virtual IEnumerable<int> GetSkipSortingDepths()
        {
            return Array.Empty<int>();
        }
    }
}
