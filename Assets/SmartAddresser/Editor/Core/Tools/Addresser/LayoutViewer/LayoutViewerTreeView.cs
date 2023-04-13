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

        private readonly Texture2D _badgeBackgroundTexture;
        private readonly Texture2D _failedTexture;
        private readonly Texture2D _successTexture;
        private readonly Texture2D _warningTexture;

        [NonSerialized] private int _currentId;

        public LayoutViewerTreeView(State state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
            ColumnStates = state.ColumnStates;
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
                            item.displayName = GetText(item, columnIndex);
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
                            GUI.Label(labelRect, GetText(item, columnIndex));
                            break;
                        case Columns.AssetPath:
                            GUI.Label(cellRect, GetText(item, columnIndex));
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
                GUI.Label(labelRect, text);

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

        protected override string GetTextForSearch(TreeViewItem item, int columnIndex)
        {
            return GetText(item, columnIndex);
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

            public State()
            {
                _columnStates = GetColumnStates();
            }

            public MultiColumnHeaderState.Column[] ColumnStates => _columnStates;

            private MultiColumnHeaderState.Column[] GetColumnStates()
            {
                var groupNameAddressColumn = new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Group Name / Address"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    width = 200,
                    minWidth = 50,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                var assetPathColumn = new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Asset Path"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    width = 200,
                    minWidth = 50,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                var labelsColumn = new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Labels"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    width = 100,
                    minWidth = 20,
                    autoResize = false,
                    allowToggleVisibility = true
                };
                var versionsColumn = new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Versions"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    width = 100,
                    minWidth = 20,
                    autoResize = false,
                    allowToggleVisibility = true
                };
                return new[] { groupNameAddressColumn, assetPathColumn, labelsColumn, versionsColumn };
            }
        }
    }
}
