using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    /// <summary>
    ///     View for the asset group collection panel.
    /// </summary>
    internal sealed class AssetGroupCollectionPanelView
    {
        private const string AddButtonName = "Add Asset Group";
        private const string PasteMenuName = "Paste Asset Group";

        private readonly Subject<Empty> _addButtonClickedSubject = new Subject<Empty>();

        private readonly List<string> _groupPanelViewOrder = new List<string>();

        private readonly ObservableDictionary<string, AssetGroupPanelView> _groupPanelViews =
            new ObservableDictionary<string, AssetGroupPanelView>();

        private readonly Subject<Empty> _pasteMenuExecutedSubject = new Subject<Empty>();

        public IObservable<Empty> AddButtonClickedAsObservable => _addButtonClickedSubject;
        public IObservable<Empty> PasteMenuExecutedAsObservable => _pasteMenuExecutedSubject;
        public IReadOnlyObservableDictionary<string, AssetGroupPanelView> GroupPanelViews => _groupPanelViews;

        public bool Enabled { get; set; }

        public event Func<bool> CanPaste;

        public void Dispose()
        {
            // Clear the group panel views.
            ClearGroupPanelViews();

            // Dispose all the disposables.
            _addButtonClickedSubject.Dispose();
            _pasteMenuExecutedSubject.Dispose();
        }

        public void DoLayout()
        {
            var enabled = GUI.enabled;
            GUI.enabled = GUI.enabled && Enabled;

            foreach (var groupId in _groupPanelViewOrder)
            {
                var groupPanelView = _groupPanelViews[groupId];
                groupPanelView.DoLayout();
            }

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

                if (CanPaste == null || CanPaste.Invoke())
                    menu.AddItem(new GUIContent(PasteMenuName), false,
                        () => _pasteMenuExecutedSubject.OnNext(Empty.Default));
                else
                    menu.AddDisabledItem(new GUIContent(PasteMenuName), false);

                menu.ShowAsContext();
            }

            GUI.enabled = enabled;
        }

        public AssetGroupPanelView AddGroupPanelView(AssetGroup group, int index = -1)
        {
            var groupPanelView = new AssetGroupPanelView();
            _groupPanelViews.Add(group.Id, groupPanelView);
            if (index == -1)
                _groupPanelViewOrder.Add(group.Id);
            else
                _groupPanelViewOrder.Insert(index, group.Id);
            return groupPanelView;
        }

        public void RemoveGroupPanelView(string groupId)
        {
            var groupView = _groupPanelViews[groupId];
            groupView.Dispose();
            _groupPanelViews.Remove(groupId);
            _groupPanelViewOrder.Remove(groupId);
        }

        public void ClearGroupPanelViews()
        {
            foreach (var groupView in _groupPanelViews.Values)
                groupView.Dispose();
            _groupPanelViews.Clear();
            _groupPanelViewOrder.Clear();
        }

        public void ChangGroupPanelViewOrder(string groupId, int newIndex)
        {
            _groupPanelViewOrder.Remove(groupId);
            _groupPanelViewOrder.Insert(newIndex, groupId);
        }
    }
}
