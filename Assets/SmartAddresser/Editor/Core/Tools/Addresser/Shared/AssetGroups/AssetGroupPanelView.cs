using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    /// <summary>
    ///     View to the asset group panel.
    /// </summary>
    internal sealed class AssetGroupPanelView : IDisposable
    {
        private const string RenameMenuName = "Rename";
        private const string RemoveMenuName = "Remove";
        private const string MoveUpMenuName = "Move Up";
        private const string MoveUpByMenuName = "Move Up By";
        private const string MoveDownMenuName = "Move Down";
        private const string MoveDownByMenuName = "Move Down By";
        private const string CopyMenuName = "Copy";
        private const string PasteMenuName = "Paste As New";
        private const string PasteValuesMenuName = "Paste Values";
        private const string PasteFilterMenuName = "Paste Filter";
        private readonly Subject<Empty> _addFilterButtonClickedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _copyGroupMenuExecutedSubject = new Subject<Empty>();

        private readonly ObservableDictionary<string, AssetFilterView> _filterViews =
            new ObservableDictionary<string, AssetFilterView>();

        private readonly List<string> _filterViewsOrder = new List<string>();

        private readonly Subject<Empty> _moveDownMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<int> _moveDownByMenuExecutedSubject = new Subject<int>();
        private readonly Subject<Empty> _moveUpMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<int> _moveUpByMenuExecutedSubject = new Subject<int>();
        private readonly Subject<string> _nameChangedSubject = new Subject<string>();
        private readonly Subject<Empty> _pasteFilterMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _pasteGroupMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _pasteGroupValuesMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _removeGroupMenuExecutedSubject = new Subject<Empty>();

        public string GroupName { get; set; }

        public bool Enabled { get; set; }
        
        public IObservable<string> NameChangedAsObservable => _nameChangedSubject;

        public IObservable<Empty> RemoveGroupMenuExecutedAsObservable => _removeGroupMenuExecutedSubject;

        public IObservable<Empty> MoveUpMenuExecutedAsObservable => _moveUpMenuExecutedSubject;

        public IObservable<int> MoveUpByMenuExecutedAsObservable => _moveUpByMenuExecutedSubject;

        public IObservable<Empty> MoveDownMenuExecutedAsObservable => _moveDownMenuExecutedSubject;

        public IObservable<int> MoveDownByMenuExecutedAsObservable => _moveDownByMenuExecutedSubject;

        public IObservable<Empty> CopyGroupMenuExecutedAsObservable => _copyGroupMenuExecutedSubject;

        public IObservable<Empty> PasteGroupMenuExecutedSubject => _pasteGroupMenuExecutedSubject;

        public IObservable<Empty> PasteGroupValuesMenuExecutedSubject => _pasteGroupValuesMenuExecutedSubject;

        public IObservable<Empty> AddFilterButtonClickedAsObservable => _addFilterButtonClickedSubject;

        public IObservable<Empty> PasteFilterMenuExecutedSubject => _pasteFilterMenuExecutedSubject;

        public IReadOnlyObservableDictionary<string, AssetFilterView> FilterViews => _filterViews;

        public void Dispose()
        {
            // Clear the filter views.
            ClearFilterViews();

            // Dispose all the disposables.
            _nameChangedSubject.Dispose();
            _addFilterButtonClickedSubject.Dispose();
            _moveUpMenuExecutedSubject.Dispose();
            _moveUpByMenuExecutedSubject.Dispose();
            _moveDownMenuExecutedSubject.Dispose();
            _moveDownByMenuExecutedSubject.Dispose();
            _copyGroupMenuExecutedSubject.Dispose();
            _pasteGroupMenuExecutedSubject.Dispose();
            _pasteGroupValuesMenuExecutedSubject.Dispose();
            _removeGroupMenuExecutedSubject.Dispose();
            _pasteFilterMenuExecutedSubject.Dispose();
        }

        public event Func<bool> CanPasteGroup;
        public event Func<bool> CanPasteGroupValues;
        public event Func<bool> CanPasteFilter;

        public event Func<ICollection<int>> GetMoveUpByOptions;
        public event Func<ICollection<int>> GetMoveDownByOptions;

        public AssetFilterView AddFilterView(IAssetFilter filter, int index = -1)
        {
            var filterView = new AssetFilterView(filter);
            _filterViews.Add(filter.Id, filterView);
            if (index == -1)
                _filterViewsOrder.Add(filter.Id);
            else
                _filterViewsOrder.Insert(index, filter.Id);
            return filterView;
        }

        public void RemoveFilterView(string filterId)
        {
            var filterView = _filterViews[filterId];
            filterView.Dispose();
            _filterViews.Remove(filterId);
            _filterViewsOrder.Remove(filterId);
        }

        public void ClearFilterViews()
        {
            foreach (var filterView in _filterViews.Values)
                filterView.Dispose();
            _filterViews.Clear();
            _filterViewsOrder.Clear();
        }

        public void ChangeFilterViewOrder(string filterId, int newIndex)
        {
            _filterViewsOrder.Remove(filterId);
            _filterViewsOrder.Insert(newIndex, filterId);
        }

        public void DoLayout()
        {
            var enabled = GUI.enabled;
            GUI.enabled = GUI.enabled && Enabled;
            
            var rect = GUILayoutUtility.GetRect(1, 20, GUILayout.ExpandWidth(true));

            // Title
            EditorGUI.DrawRect(rect, EditorGUIUtil.TitleBackgroundColor);
            rect.xMin += 4;
            var titleRect = rect;
            titleRect.xMax -= 20;
            EditorGUI.LabelField(rect, GroupName, EditorStyles.boldLabel);
            var titleButtonRect = rect;
            titleButtonRect.xMin += titleButtonRect.width - 20;

            // Add Filter Button
            var plusIconTexture = EditorGUIUtility.IconContent(EditorGUIUtil.ToolbarPlusIconName).image;
            GUI.DrawTexture(titleButtonRect, plusIconTexture, ScaleMode.StretchToFill);
            if (GUI.Button(titleButtonRect, "", GUIStyle.none))
                _addFilterButtonClickedSubject.OnNext(Empty.Default);

            // Right Click Menu
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 &&
                titleRect.Contains(Event.current.mousePosition))
            {
                var menu = new GenericMenu();
                var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

                // Rename
                menu.AddItem(new GUIContent(RenameMenuName), false,
                    () =>
                    {
                        TextFieldPopup.Show(mousePos, GroupName, null, x => _nameChangedSubject.OnNext(x),
                            RenameMenuName);
                    });

                // Remove
                menu.AddItem(new GUIContent(RemoveMenuName), false,
                    () => _removeGroupMenuExecutedSubject.OnNext(Empty.Default));

                // Move Up
                menu.AddItem(new GUIContent(MoveUpMenuName), false,
                    () => _moveUpMenuExecutedSubject.OnNext(Empty.Default));

                // Move Up By
                var moveUpByList = GetMoveUpByOptions?.Invoke();
                if (moveUpByList == null || moveUpByList.Count == 0)
                    menu.AddDisabledItem(new GUIContent(MoveUpByMenuName), false);
                else
                    foreach (var count in moveUpByList)
                        menu.AddItem(new GUIContent($"{MoveUpByMenuName}/{count}"), false,
                                     () => _moveUpByMenuExecutedSubject.OnNext(count));

                // Move Down
                menu.AddItem(new GUIContent(MoveDownMenuName), false,
                    () => _moveDownMenuExecutedSubject.OnNext(Empty.Default));

                // Move Down By
                var moveDownByList = GetMoveDownByOptions?.Invoke();
                if (moveDownByList == null || moveDownByList.Count == 0)
                    menu.AddDisabledItem(new GUIContent(MoveDownByMenuName), false);
                else
                    foreach (var count in moveDownByList)
                        menu.AddItem(new GUIContent($"{MoveDownByMenuName}/{count}"), false,
                                     () => _moveDownByMenuExecutedSubject.OnNext(count));

                // Copy Group
                menu.AddItem(new GUIContent(CopyMenuName), false,
                    () => _copyGroupMenuExecutedSubject.OnNext(Empty.Default));

                // Paste
                if (CanPasteGroup == null || CanPasteGroup.Invoke())
                    menu.AddItem(new GUIContent(PasteMenuName), false,
                        () => _pasteGroupMenuExecutedSubject.OnNext(Empty.Default));
                else
                    menu.AddDisabledItem(new GUIContent(PasteMenuName), false);

                // Paste Values
                if (CanPasteGroupValues == null || CanPasteGroupValues.Invoke())
                    menu.AddItem(new GUIContent(PasteValuesMenuName), false,
                        () => _pasteGroupValuesMenuExecutedSubject.OnNext(Empty.Default));
                else
                    menu.AddDisabledItem(new GUIContent(PasteValuesMenuName), false);

                // Paste Filter
                if (CanPasteFilter == null || CanPasteFilter.Invoke())
                    menu.AddItem(new GUIContent(PasteFilterMenuName), false,
                        () => _pasteFilterMenuExecutedSubject.OnNext(Empty.Default));
                else
                    menu.AddDisabledItem(new GUIContent(PasteFilterMenuName), false);

                menu.ShowAsContext();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(8);

            foreach (var filterId in _filterViewsOrder)
            {
                var filterView = _filterViews[filterId];
                filterView.DoLayout();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            // Border
            var borderRect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(borderRect, EditorGUIUtil.EditorBorderColor);

            GUI.enabled = enabled;
        }
    }
}
