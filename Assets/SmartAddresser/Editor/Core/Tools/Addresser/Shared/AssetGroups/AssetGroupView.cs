using System;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    /// <summary>
    ///     View to draw <see cref="AssetGroup" />.
    /// </summary>
    internal sealed class AssetGroupView : IDisposable
    {
        private const string RenameMenuName = "Rename";
        private const string RemoveMenuName = "Remove";
        private const string MoveUpMenuName = "Move Up";
        private const string MoveDownMenuName = "Move Down";
        private const string CopyMenuName = "Copy";
        private const string PasteMenuName = "Paste As New";
        private const string PasteValuesMenuName = "Paste Values";
        private const string PasteFilterMenuName = "Paste Filter";
        private readonly Subject<Empty> _addFilterButtonClickedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _copyGroupMenuExecutedSubject = new Subject<Empty>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly ObservableDictionary<string, AssetFilterView> _filterViews =
            new ObservableDictionary<string, AssetFilterView>();

        private readonly Subject<Empty> _moveDownMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _moveUpMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<string> _nameChangedSubject = new Subject<string>();
        private readonly Subject<Empty> _pasteFilterMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _pasteGroupMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _pasteGroupValuesMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _removeGroupMenuExecutedSubject = new Subject<Empty>();

        public AssetGroupView(AssetGroup group)
        {
            Group = group;

            foreach (var filter in group.Filters)
                AddFilter(filter);

            group.Filters.ObservableAdd.Subscribe(x => AddFilter(x.Value)).DisposeWith(_disposables);
            group.Filters.ObservableRemove.Subscribe(x => RemoveFilter(x.Value)).DisposeWith(_disposables);
            group.Filters.ObservableClear.Subscribe(_ => ClearFilters()).DisposeWith(_disposables);
        }

        public AssetGroup Group { get; }

        public IObservable<string> NameChangedAsObservable => _nameChangedSubject;

        public IObservable<Empty> RemoveGroupMenuExecutedAsObservable => _removeGroupMenuExecutedSubject;

        public IObservable<Empty> MoveUpMenuExecutedAsObservable => _moveUpMenuExecutedSubject;

        public IObservable<Empty> MoveDownMenuExecutedAsObservable => _moveDownMenuExecutedSubject;

        public IObservable<Empty> CopyGroupMenuExecutedAsObservable => _copyGroupMenuExecutedSubject;

        public IObservable<Empty> PasteGroupMenuExecutedSubject => _pasteGroupMenuExecutedSubject;

        public IObservable<Empty> PasteGroupValuesMenuExecutedSubject => _pasteGroupValuesMenuExecutedSubject;

        public IObservable<Empty> AddFilterButtonClickedAsObservable => _addFilterButtonClickedSubject;

        public IObservable<Empty> PasteFilterMenuExecutedSubject => _pasteFilterMenuExecutedSubject;

        public IReadOnlyObservableDictionary<string, AssetFilterView> FilterViews => _filterViews;

        public void Dispose()
        {
            // Dispose the filter views.
            foreach (var filterView in _filterViews)
                filterView.Value.Dispose();
            _filterViews.Clear();

            // Dispose all the disposables.
            _disposables.Dispose();
            _nameChangedSubject.Dispose();
            _addFilterButtonClickedSubject.Dispose();
            _moveUpMenuExecutedSubject.Dispose();
            _moveDownMenuExecutedSubject.Dispose();
            _copyGroupMenuExecutedSubject.Dispose();
            _pasteGroupMenuExecutedSubject.Dispose();
            _pasteGroupValuesMenuExecutedSubject.Dispose();
            _removeGroupMenuExecutedSubject.Dispose();
            _moveUpMenuExecutedSubject.Dispose();
            _moveDownMenuExecutedSubject.Dispose();
            _pasteFilterMenuExecutedSubject.Dispose();
        }

        public event Func<bool> CanPasteGroup;
        public event Func<bool> CanPasteGroupValues;
        public event Func<bool> CanPasteFilter;

        public void DoLayout()
        {
            var rect = GUILayoutUtility.GetRect(1, 20, GUILayout.ExpandWidth(true));

            // Title
            EditorGUI.DrawRect(rect, EditorGUIUtil.TitleBackgroundColor);
            rect.xMin += 4;
            var titleRect = rect;
            titleRect.xMax -= 20;
            EditorGUI.LabelField(rect, Group.Name.Value, EditorStyles.boldLabel);
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
                        TextFieldPopup.Show(mousePos, Group.Name.Value, null, x => _nameChangedSubject.OnNext(x),
                            RenameMenuName);
                    });

                // Remove
                menu.AddItem(new GUIContent(RemoveMenuName), false,
                    () => _removeGroupMenuExecutedSubject.OnNext(Empty.Default));

                // Move Up
                menu.AddItem(new GUIContent(MoveUpMenuName), false,
                    () => _moveUpMenuExecutedSubject.OnNext(Empty.Default));

                // Move Down
                menu.AddItem(new GUIContent(MoveDownMenuName), false,
                    () => _moveDownMenuExecutedSubject.OnNext(Empty.Default));

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

            // Draw the Filters in the same order as model.
            foreach (var filter in Group.Filters)
                _filterViews[filter.Id].DoLayout();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            // Border
            var borderRect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(borderRect, EditorGUIUtil.EditorBorderColor);
        }

        private void AddFilter(IAssetFilter filter)
        {
            var filterView = new AssetFilterView(filter);
            _filterViews.Add(filter.Id, filterView);
        }

        private void RemoveFilter(IAssetFilter filter)
        {
            var filterView = _filterViews[filter.Id];
            filterView.Dispose();
            _filterViews.Remove(filter.Id);
        }

        private void ClearFilters()
        {
            foreach (var filterView in _filterViews.Values)
                filterView.Dispose();
            _filterViews.Clear();
        }
    }
}
