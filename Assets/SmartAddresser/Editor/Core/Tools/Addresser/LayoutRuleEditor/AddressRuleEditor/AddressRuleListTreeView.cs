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

        [NonSerialized] private int _currentId;

        public AddressRuleListTreeView(State state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
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
                    GUI.Label(cellRect, GetText(item, columnIndex));
                    break;
                case Columns.Control:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly;
                    cellRect.x += cellRect.width / 2 - 7;
                    cellRect.width = 14;
                    item.Rule.Control.Value = GUI.Toggle(cellRect, item.Rule.Control.Value, "");
                    break;
                case Columns.AssetGroups:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly && item.Rule.Control.Value;
                    GUI.Label(cellRect, GetText(item, columnIndex));
                    break;
                case Columns.AddressRule:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly && item.Rule.Control.Value;
                    GUI.Label(cellRect, GetText(item, columnIndex));
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
                    return item.Rule.Control.ToString();
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
        public sealed class State : StateBase
        {
            protected override MultiColumnHeaderState.Column[] GetColumnStates()
            {
                var groupsColumn = new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Groups"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    width = 150,
                    minWidth = 50,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                var controlColumn = new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Control"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    width = 60,
                    minWidth = 60,
                    maxWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                var assetGroupsColumn = new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Asset Groups"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    width = 200,
                    minWidth = 50,
                    autoResize = true,
                    allowToggleVisibility = false
                };
                var addressRuleColumn = new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Address Rule"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    width = 200,
                    minWidth = 50,
                    autoResize = true,
                    allowToggleVisibility = false
                };
                return new[] { groupsColumn, controlColumn, assetGroupsColumn, addressRuleColumn };
            }
        }
    }
}
