using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Foundation.EasyTreeView;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     Tree view for the Address Rule Editor.
    /// </summary>
    internal sealed class AddressRuleListTreeView : TreeViewBase
    {
        public enum Columns
        {
            Groups,
            Control,
            AssetGroups,
            AddressRule
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

        public AddressRuleListTreeView(State state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
            ColumnStates = state.GetColumnStates();
            rowHeight = 16;
            Reload();
        }

        public Item AddItem(AddressRule rule, int index = -1)
        {
            rule.RefreshAddressProviderDescription();
            rule.RefreshAssetGroupDescription();
            var item = new Item(rule)
            {
                id = _currentId++
            };
            AddItemAndSetParent(item, -1, index);
            return item;
        }

        protected override void CellGUI(int columnIndex, Rect cellRect, RowGUIArgs args)
        {
            var item = (Item)args.item;
            var addressableGroup = item.Rule.AddressableGroup;
            switch ((Columns)columnIndex)
            {
                case Columns.Groups:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly && item.Rule.Control.Value;
                    GUI.Label(cellRect, GetText(item, columnIndex), CellLabelStyle);
                    break;
                case Columns.Control:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly;
                    cellRect.x += cellRect.width / 2 - 7;
                    cellRect.width = 14;
                    item.Rule.Control.Value = GUI.Toggle(cellRect, item.Rule.Control.Value, "");
                    break;
                case Columns.AssetGroups:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly && item.Rule.Control.Value;
                    GUI.Label(cellRect, GetText(item, columnIndex), CellLabelStyle);
                    break;
                case Columns.AddressRule:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly && item.Rule.Control.Value;
                    GUI.Label(cellRect, GetText(item, columnIndex), CellLabelStyle);
                    break;
                default:
                    throw new NotImplementedException();
            }

            GUI.enabled = true;
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

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        protected override bool CanBeParent(TreeViewItem item)
        {
            return false;
        }

        private string GetText(Item item, int columnIndex)
        {
            switch ((Columns)columnIndex)
            {
                case Columns.Groups:
                    return item.Rule.AddressableGroup == null
                        ? "[Missing Reference]"
                        : item.Rule.AddressableGroup.name;
                case Columns.Control:
                    return item.Rule.Control.Value.ToString();
                case Columns.AssetGroups:
                    if (GetSelection().FirstOrDefault() == item.id)
                        item.Rule.RefreshAssetGroupDescription();
                    return item.Rule.AssetGroupDescription.Value;
                case Columns.AddressRule:
                    if (GetSelection().FirstOrDefault() == item.id)
                        item.Rule.RefreshAddressProviderDescription();
                    return item.Rule.AddressProviderDescription.Value;
                default:
                    throw new NotImplementedException();
            }
        }

        public sealed class Item : TreeViewItem
        {
            public Item(AddressRule rule)
            {
                Rule = rule;
            }

            public AddressRule Rule { get; }
        }

        [Serializable]
        public sealed class State : TreeViewState
        {
            [SerializeField] private MultiColumnHeaderState.Column[] _columnStates;

            public MultiColumnHeaderState.Column[] GetColumnStates()
            {
                var oldGroupsColumn = _columnStates?[0];
                var oldControlColumn = _columnStates?[1];
                var oldAssetGroupsColumn = _columnStates?[2];
                var oldAddressRuleColumn = _columnStates?[3];
                
                var groupsColumn = new MultiColumnHeaderState.Column
                {
                    width = oldGroupsColumn?.width ?? 150,
                    sortedAscending = oldGroupsColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Groups"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    minWidth = 50,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                var controlColumn = new MultiColumnHeaderState.Column
                {
                    width = oldControlColumn?.width ?? 60,
                    sortedAscending = oldControlColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Control"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    minWidth = 60,
                    maxWidth = 60,
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
                var addressRuleColumn = new MultiColumnHeaderState.Column
                {
                    width = oldAddressRuleColumn?.width ?? 200,
                    sortedAscending = oldAddressRuleColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Address Rule"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    minWidth = 50,
                    autoResize = true,
                    allowToggleVisibility = false
                };
                _columnStates = new[] { groupsColumn, controlColumn, assetGroupsColumn, addressRuleColumn };
                return _columnStates;
            }
        }
    }
}
