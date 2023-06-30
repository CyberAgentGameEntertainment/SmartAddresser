using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Foundation.EasyTreeView;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     Tree view for the Version Rule Editor.
    /// </summary>
    internal sealed class VersionRuleListTreeView : TreeViewBase
    {
        private static string DragType => nameof(VersionRuleListTreeView);
        
        public enum Columns
        {
            Name,
            AssetGroups,
            VersionRule
        }
        
        private GUIStyle _cellLabelStyle;

        private GUIStyle CellLabelStyle
        {
            get
            {
                if (_cellLabelStyle == null)
                {
                    _cellLabelStyle = new GUIStyle(EditorStyles.label)
                    {
                        wordWrap = false
                    };
                }
                
                return _cellLabelStyle;
            }
        }

        [NonSerialized] private int _currentId;

        private readonly Subject<(Item item, int newIndex)> _itemIndexChangedSubject
            = new Subject<(Item item, int newIndex)>();

        public IObservable<(Item item, int newIndex)> ItemIndexChangedAsObservable =>
            _itemIndexChangedSubject;

        public VersionRuleListTreeView(State state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
            ColumnStates = state.GetColumnStates();
            rowHeight = 16;
            Reload();
        }

        public Item AddItem(VersionRule rule, int index = -1)
        {
            rule.RefreshVersionProviderDescription();
            rule.RefreshAssetGroupDescription();
            var item = new Item(rule)
            {
                id = _currentId++
            };
            item.displayName = rule.Name.Value;
            AddItemAndSetParent(item, -1, index);
            return item;
        }

        protected override void CellGUI(int columnIndex, Rect cellRect, RowGUIArgs args)
        {
            var item = (Item)args.item;
            switch ((Columns)columnIndex)
            {
                case Columns.Name:
                    args.rowRect = cellRect;
                    base.CellGUI(columnIndex, cellRect, args);
                    break;
                case Columns.AssetGroups:
                    GUI.Label(cellRect, GetText(item, columnIndex), CellLabelStyle);
                    break;
                case Columns.VersionRule:
                    GUI.Label(cellRect, GetText(item, columnIndex), CellLabelStyle);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override IOrderedEnumerable<TreeViewItem> OrderItems(IList<TreeViewItem> items, int keyColumnIndex,
            bool ascending)
        {
            string KeySelector(TreeViewItem x)
            {
                return GetText((Item)x, keyColumnIndex);
            }

            return ascending
                ? items.OrderBy(KeySelector, Comparer<string>.Create(EditorUtility.NaturalCompare))
                : items.OrderByDescending(KeySelector, Comparer<string>.Create(EditorUtility.NaturalCompare));
        }

        protected override string GetTextForSearch(TreeViewItem item, int columnIndex)
        {
            return GetText((Item)item, columnIndex);
        }

        protected override bool CanRename(TreeViewItem item)
        {
            return true;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            if (args.acceptedRename)
            {
                var item = (Item)GetItem(args.itemID);
                item.Rule.Name.Value = args.newName;
                item.displayName = args.newName;
                Reload();
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        protected override bool CanBeParent(TreeViewItem item)
        {
            return false;
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return string.IsNullOrEmpty(searchString);
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            var selections = args.draggedItemIDs;
            if (selections.Count <= 0) return;

            var items = GetRows()
                .Where(i => selections.Contains(i.id))
                .Select(x => (Item)x)
                .ToArray();

            if (items.Length <= 0) return;

            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData(DragType, items);
            DragAndDrop.StartDrag(items.Length > 1 ? "<Multiple>" : items[0].displayName);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (args.performDrop)
            {
                var data = DragAndDrop.GetGenericData(DragType);
                var items = (Item[])data;

                if (items == null || items.Length <= 0)
                    return DragAndDropVisualMode.None;

                switch (args.dragAndDropPosition)
                {
                    case DragAndDropPosition.BetweenItems:
                        var afterIndex = args.insertAtIndex;
                        foreach (var item in items)
                        {
                            var itemIndex = RootItem.children.IndexOf(item);
                            if (itemIndex < afterIndex) afterIndex--;

                            SetItemIndex(item.id, afterIndex, true);
                            afterIndex++;
                        }

                        SetSelection(items.Select(x => x.id).ToArray());

                        Reload();
                        break;
                    case DragAndDropPosition.UponItem:
                    case DragAndDropPosition.OutsideItems:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return DragAndDropVisualMode.Move;
        }

        public void SetItemIndex(int itemId, int index, bool notify)
        {
            var item = (Item)GetItem(itemId);
            var children = RootItem.children;
            var itemIndex = RootItem.children.IndexOf(item);
            children.RemoveAt(itemIndex);
            children.Insert(index, item);
            if (notify) _itemIndexChangedSubject.OnNext((item, index));
        }

        private string GetText(Item item, int columnIndex)
        {
            switch ((Columns)columnIndex)
            {
                case Columns.Name:
                    return item.Rule.Name.Value;
                case Columns.AssetGroups:
                    if (GetSelection().FirstOrDefault() == item.id)
                        item.Rule.RefreshAssetGroupDescription();
                    return item.Rule.AssetGroupDescription.Value;
                case Columns.VersionRule:
                    if (GetSelection().FirstOrDefault() == item.id)
                        item.Rule.RefreshVersionProviderDescription();
                    return item.Rule.VersionProviderDescription.Value;
                default:
                    throw new NotImplementedException();
            }
        }

        public sealed class Item : TreeViewItem
        {
            public Item(VersionRule rule)
            {
                Rule = rule;
            }

            public VersionRule Rule { get; }
        }

        [Serializable]
        public sealed class State : TreeViewState
        {
            [SerializeField] private MultiColumnHeaderState.Column[] _columnStates;

            public MultiColumnHeaderState.Column[] GetColumnStates()
            {
                var oldNameColumn = _columnStates?[0];
                var oldAssetGroupsColumn = _columnStates?[1];
                var oldVersionRuleColumn = _columnStates?[2];
                
                var nameColumn = new MultiColumnHeaderState.Column
                {
                    width = oldNameColumn?.width ?? 150,
                    sortedAscending = oldNameColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    minWidth = 50,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                var assetGroupsColumn = new MultiColumnHeaderState.Column
                {
                    width = oldAssetGroupsColumn?.width ?? 200,
                    sortedAscending = oldAssetGroupsColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Asset Groups"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    minWidth = 50,
                    autoResize = true,
                    allowToggleVisibility = false
                };
                var versionRuleColumn = new MultiColumnHeaderState.Column
                {
                    width = oldVersionRuleColumn?.width ?? 200,
                    sortedAscending = oldVersionRuleColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Version Rule"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    minWidth = 50,
                    autoResize = true,
                    allowToggleVisibility = false
                };
                _columnStates = new[] { nameColumn, assetGroupsColumn, versionRuleColumn };
                return _columnStates;
            }
        }
    }
}
