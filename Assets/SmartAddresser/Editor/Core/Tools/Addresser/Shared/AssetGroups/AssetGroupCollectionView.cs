using System;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    /// <summary>
    ///     View to draw the collection of the <see cref="AssetGroup" />.
    /// </summary>
    internal sealed class AssetGroupCollectionView
    {
        private const string AddButtonName = "Add Asset Group";
        private const string PasteMenuName = "Paste Asset Group";

        private readonly Subject<Empty> _addButtonClickedSubject = new Subject<Empty>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly ObservableDictionary<string, AssetGroupView> _groupViews =
            new ObservableDictionary<string, AssetGroupView>();

        private readonly Subject<Empty> _pasteMenuExecutedSubject = new Subject<Empty>();

        public AssetGroupCollectionView(IReadOnlyObservableList<AssetGroup> groups)
        {
            Groups = groups;

            foreach (var group in groups)
                AddGroup(group);

            groups.ObservableAdd.Subscribe(x => AddGroup(x.Value)).DisposeWith(_disposables);
            groups.ObservableRemove.Subscribe(x => RemoveGroup(x.Value)).DisposeWith(_disposables);
            groups.ObservableClear.Subscribe(x => ClearGroups()).DisposeWith(_disposables);
        }

        public IObservable<Empty> AddButtonClickedAsObservable => _addButtonClickedSubject;
        public IObservable<Empty> PasteMenuExecutedAsObservable => _pasteMenuExecutedSubject;
        public IReadOnlyObservableList<AssetGroup> Groups { get; }
        public IReadOnlyObservableDictionary<string, AssetGroupView> GroupViews => _groupViews;
        public event Func<bool> CanPaste;

        public void Dispose()
        {
            ClearGroups();

            _disposables.Dispose();
            _addButtonClickedSubject.Dispose();
            _pasteMenuExecutedSubject.Dispose();
        }

        public void DoLayout()
        {
            // Draw the Groups in the same order as model.
            foreach (var group in Groups)
                _groupViews[group.Id].DoLayout();

            var bottomRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight + 8,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            var buttonRect = bottomRect;
            buttonRect.height = EditorGUIUtility.singleLineHeight;
            buttonRect.y += 4;
            buttonRect.x = buttonRect.width / 2.0f - 60;
            buttonRect.width = 120;
            if (GUI.Button(buttonRect, AddButtonName))
                _addButtonClickedSubject.OnNext(Empty.Default);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 &&
                bottomRect.Contains(Event.current.mousePosition))
            {
                var menu = new GenericMenu();

                if (CanPaste != null && CanPaste.Invoke())
                    menu.AddItem(new GUIContent(PasteMenuName), false,
                        () => _pasteMenuExecutedSubject.OnNext(Empty.Default));
                else
                    menu.AddDisabledItem(new GUIContent(PasteMenuName), false);

                menu.ShowAsContext();
            }
        }

        private void AddGroup(AssetGroup group)
        {
            var groupView = new AssetGroupView(group);
            _groupViews.Add(group.Id, groupView);
        }

        private void RemoveGroup(AssetGroup group)
        {
            var groupView = _groupViews[group.Id];
            groupView.Dispose();
            _groupViews.Remove(group.Id);
        }

        private void ClearGroups()
        {
            foreach (var groupView in _groupViews.Values)
                groupView.Dispose();
            _groupViews.Clear();
        }
    }
}
