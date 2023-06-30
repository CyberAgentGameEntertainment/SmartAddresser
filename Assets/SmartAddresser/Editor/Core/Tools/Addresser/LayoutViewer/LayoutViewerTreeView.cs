using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.Layouts;
using SmartAddresser.Editor.Foundation.EasyTreeView;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer
{
    /// <summary>
    ///     Tree view for the Address Viewer.
    /// </summary>
    internal sealed class LayoutViewerTreeView : TreeViewBase
    {
        public enum Columns
        {
            GroupNameOrAddress,
            AssetPath,
            Labels,
            Versions
        }
        
        private static readonly int[] DefaultSkipSortingDepths = { 0 };
        
        private readonly Texture2D _badgeBackgroundTexture;
        private readonly Texture2D _failedTexture;
        private readonly Texture2D _successTexture;
        private readonly Texture2D _warningTexture;
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

        public LayoutViewerTreeView(State state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
            ColumnStates = state.GetColumnStates();
            var badgeBgTexture = new Texture2D(1, 1);
            badgeBgTexture.SetPixel(0, 0, new Color32(34, 73, 128, 255));
            badgeBgTexture.Apply();
            _badgeBackgroundTexture = badgeBgTexture;
            _failedTexture = (Texture2D)EditorGUIUtility.Load("TestFailed");
            _warningTexture = (Texture2D)EditorGUIUtility.Load("Warning");
            _successTexture = (Texture2D)EditorGUIUtility.Load("TestPassed");
            Reload();
        }

        public GroupItem AddGroup(Group group)
        {
            var groupItem = new GroupItem(group)
            {
                id = _currentId++
            };
            AddItemAndSetParent(groupItem, -1);

            foreach (var entry in group.Entries)
            {
                var entryItem = new EntryItem(entry)
                {
                    id = _currentId++
                };
                AddItemAndSetParent(entryItem, groupItem.id);
            }

            return groupItem;
        }

        protected override void CellGUI(int columnIndex, Rect cellRect, RowGUIArgs args)
        {
            var errorType = GetLayoutErrorType(args.item);
            var errorTypeIcon = GetErrorTypeIcon(errorType);

            var item = args.item;
            switch (item)
            {
                case GroupItem _:
                    switch ((Columns)columnIndex)
                    {
                        case Columns.GroupNameOrAddress:
                            item.displayName = GetTextForDisplay(item, columnIndex);
                            item.icon = errorTypeIcon;
                            base.CellGUI(columnIndex, cellRect, args);
                            break;
                        case Columns.AssetPath:
                        case Columns.Labels:
                        case Columns.Versions:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, null);
                    }

                    break;
                case EntryItem entryItem:
                    switch ((Columns)columnIndex)
                    {
                        case Columns.GroupNameOrAddress:
                            cellRect.xMin += GetContentIndent(item);
                            var labelRect = cellRect;
                            var statusIconRect = cellRect;
                            statusIconRect.width = statusIconRect.height;
                            labelRect.xMin += statusIconRect.width + 2.0f;
                            GUI.DrawTexture(statusIconRect, errorTypeIcon);
                            GUI.Label(labelRect, GetTextForDisplay(item, columnIndex), CellLabelStyle);
                            break;
                        case Columns.AssetPath:
                            GUI.Label(cellRect, GetTextForDisplay(item, columnIndex), CellLabelStyle);
                            break;
                        case Columns.Labels:
                            DrawBadges(entryItem.Entry.Labels, new Vector2(cellRect.x, cellRect.y + 1),
                                cellRect.height - 2, cellRect.width);
                            break;
                        case Columns.Versions:
                            DrawBadges(entryItem.Entry.Versions, new Vector2(cellRect.x, cellRect.y + 1),
                                cellRect.height - 2, cellRect.width);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    break;
            }
        }

        private void DrawBadges(IEnumerable<string> texts, Vector2 startPos, float height, float totalWidth)
        {
            if (texts == null)
                return;

            const float horizontalPadding = 12;
            var pos = startPos;
            var labelPosMax = startPos.x + totalWidth - horizontalPadding;
            foreach (var text in texts)
            {
                if (pos.x - startPos.x > totalWidth)
                    break;

                var labelWidth = EditorStyles.label.CalcSize(new GUIContent(text)).x + 2;
                var cornerRadius = pos.x + labelWidth + horizontalPadding - startPos.x > totalWidth
                    ? new Vector4(2, 0, 0, 2)
                    : Vector4.one * 2;
                labelWidth = Mathf.Min(labelPosMax - pos.x, labelWidth);
                var textureWidth = labelWidth + horizontalPadding;
                var textureRect = new Rect(pos.x, pos.y, textureWidth, height);
                GUI.DrawTexture(textureRect, _badgeBackgroundTexture, ScaleMode.StretchToFill, true, 0, Color.white,
                    Vector4.zero, cornerRadius);
                var labelRect = new Rect(pos.x + 6, pos.y, labelWidth, height);
                GUI.Label(labelRect, text, CellLabelStyle);

                pos.x += textureRect.width;
                pos.x += 3; // space between badges
            }
        }

        protected override IOrderedEnumerable<TreeViewItem> OrderItems(IList<TreeViewItem> items, int keyColumnIndex,
            bool ascending)
        {
            string KeySelector(TreeViewItem x)
            {
                return GetText(x, keyColumnIndex);
            }

            return ascending
                ? items.OrderBy(KeySelector, Comparer<string>.Create(EditorUtility.NaturalCompare))
                : items.OrderByDescending(KeySelector, Comparer<string>.Create(EditorUtility.NaturalCompare));
        }

        private string GetTextForDisplay(TreeViewItem item, int columnIndex)
        {
            return hasSearch ? GetTextForSearch(item, columnIndex) : GetText(item, columnIndex);
        }

        protected override string GetTextForSearch(TreeViewItem item, int columnIndex)
        {
            // GroupItem is not a search target.
            if (item is GroupItem)
                return null;

            if (columnIndex != (int)Columns.GroupNameOrAddress)
                return GetText(item, columnIndex);

            var groupItem = item.parent;
            var groupText = GetText(groupItem, columnIndex);
            var entryText = GetText(item, columnIndex);
            return $"[{groupText}] {entryText}";

        }

        protected override IEnumerable<int> GetSkipSortingDepths()
        {
            // Exclude GroupItem from sorting targets.
            return DefaultSkipSortingDepths;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override bool CanBeParent(TreeViewItem item)
        {
            return item is GroupItem;
        }

        private static LayoutErrorType GetLayoutErrorType(TreeViewItem treeViewItem)
        {
            switch (treeViewItem)
            {
                case GroupItem groupItem:
                    return groupItem.Group.ErrorType;
                case EntryItem entryItem:
                    return entryItem.Entry.ErrorType;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static string GetText(TreeViewItem item, int columnIndex)
        {
            return item switch
            {
                GroupItem groupItem => (Columns)columnIndex switch
                {
                    Columns.GroupNameOrAddress => groupItem.Group.DisplayName,
                    Columns.AssetPath => null,
                    Columns.Labels => null,
                    Columns.Versions => null,
                    _ => throw new ArgumentOutOfRangeException()
                },
                EntryItem entryItem => (Columns)columnIndex switch
                {
                    Columns.GroupNameOrAddress => entryItem.Entry.Address,
                    Columns.AssetPath => entryItem.Entry.AssetPath,
                    Columns.Labels => entryItem.Entry.Labels != null
                        ? string.Join(", ", entryItem.Entry.Labels)
                        : null,
                    Columns.Versions => entryItem.Entry.Versions != null
                        ? string.Join(", ", entryItem.Entry.Versions)
                        : null,
                    _ => throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, null)
                },
                _ => null
            };
        }

        private Texture2D GetErrorTypeIcon(LayoutErrorType errorType)
        {
            switch (errorType)
            {
                case LayoutErrorType.None:
                    return _successTexture;
                case LayoutErrorType.Warning:
                    return _warningTexture;
                case LayoutErrorType.Error:
                    return _failedTexture;
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorType), errorType, null);
            }
        }

        public sealed class GroupItem : TreeViewItem
        {
            public GroupItem(Group group)
            {
                Group = group;
            }

            public Group Group { get; }
        }

        public sealed class EntryItem : TreeViewItem
        {
            public EntryItem(Entry entry)
            {
                Entry = entry;
            }

            public Entry Entry { get; }
        }

        [Serializable]
        public sealed class State : TreeViewState
        {
            [SerializeField] private MultiColumnHeaderState.Column[] _columnStates;

            public MultiColumnHeaderState.Column[] GetColumnStates()
            {
                var oldGroupNameAddressColumn = _columnStates?[0];
                var oldAssetPathColumn = _columnStates?[1];
                var oldLabelsColumn = _columnStates?[2];
                var oldVersionsColumn = _columnStates?[3];

                var groupNameAddressColumn = new MultiColumnHeaderState.Column
                {
                    width = oldGroupNameAddressColumn?.width ?? 200,
                    sortedAscending = oldGroupNameAddressColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Group Name / Address"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = true,
                    minWidth = 50,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                var assetPathColumn = new MultiColumnHeaderState.Column
                {
                    width = oldAssetPathColumn?.width ?? 200,
                    sortedAscending = oldAssetPathColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Asset Path"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = true,
                    minWidth = 50,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                var labelsColumn = new MultiColumnHeaderState.Column
                {
                    width = oldLabelsColumn?.width ?? 100,
                    sortedAscending = oldLabelsColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Labels"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = true,
                    minWidth = 20,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                var versionsColumn = new MultiColumnHeaderState.Column
                {
                    width = oldVersionsColumn?.width ?? 100,
                    sortedAscending = oldVersionsColumn?.sortedAscending ?? true,
                    headerContent = new GUIContent("Versions"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = true,
                    minWidth = 20,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                _columnStates = new[] { groupNameAddressColumn, assetPathColumn, labelsColumn, versionsColumn };
                return _columnStates;
            }
        }
    }
}
