using System;
using System.Collections.Generic;
using System.Reflection;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared.AssetGroups
{
    /// <summary>
    ///     View to draw <see cref="IAssetFilter" />.
    /// </summary>
    internal sealed class AssetFilterView : IDisposable
    {
        private const string RemoveMenuName = "Remove Filter";
        private const string MoveUpMenuName = "Move Up Filter";
        private const string MoveUpByMenuName = "Move Up Filter By";
        private const string PasteMenuName = "Paste Filter As New";
        private const string PasteValuesMenuName = "Paste Filter Values";
        private const string MoveDownMenuName = "Move Down Filter";
        private const string MoveDownByMenuName = "Move Down Filter By";
        private const string CopyMenuName = "Copy Filter";

        private readonly Subject<Empty> _copyMenuExecutedSubject = new Subject<Empty>();
        private readonly ICustomDrawer _drawer;
        private readonly Subject<Empty> _mouseButtonClickedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _moveDownMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<int>  _moveDownByMenuExecutedSubject = new Subject<int>();
        private readonly Subject<Empty> _moveUpMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<int>  _moveUpByMenuExecutedSubject = new Subject<int>();
        private readonly Subject<Empty> _pasteMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _pasteValuesMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _removeMenuExecutedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _valueChangedSubject = new Subject<Empty>();

        public AssetFilterView(IAssetFilter filter)
        {
            Filter = filter;
            var drawer = CustomDrawerFactory.Create(filter.GetType());
            if (drawer == null)
            {
                Debug.LogError($"Drawer of {filter.GetType().Name} is not found.");
                return;
            }

            drawer.Setup(filter);

            _drawer = drawer;
        }

        public IAssetFilter Filter { get; }

        public IObservable<Empty> RemoveMenuExecutedAsObservable => _removeMenuExecutedSubject;
        public IObservable<Empty> MoveUpMenuExecutedAsObservable => _moveUpMenuExecutedSubject;
        public IObservable<int> MoveUpByMenuExecutedAsObservable => _moveUpByMenuExecutedSubject;
        public IObservable<Empty> MoveDownMenuExecutedAsObservable => _moveDownMenuExecutedSubject;
        public IObservable<int> MoveDownByMenuExecutedAsObservable => _moveDownByMenuExecutedSubject;
        public IObservable<Empty> ValueChangedAsObservable => _valueChangedSubject;
        public IObservable<Empty> CopyMenuExecutedAsObservable => _copyMenuExecutedSubject;
        public IObservable<Empty> PasteMenuExecutedSubject => _pasteMenuExecutedSubject;
        public IObservable<Empty> PasteValuesMenuExecutedSubject => _pasteValuesMenuExecutedSubject;
        public IObservable<Empty> MouseButtonClickedAsObservable => _mouseButtonClickedSubject;

        public void Dispose()
        {
            _valueChangedSubject.Dispose();
            _moveUpMenuExecutedSubject.Dispose();
            _moveDownMenuExecutedSubject.Dispose();
            _pasteMenuExecutedSubject.Dispose();
            _pasteValuesMenuExecutedSubject.Dispose();
            _removeMenuExecutedSubject.Dispose();
            _copyMenuExecutedSubject.Dispose();
            _mouseButtonClickedSubject.Dispose();
        }

        public event Func<bool> CanPaste;
        public event Func<bool> CanPasteValues;

        public event Func<ICollection<int>> GetMoveUpByOptions;
        public event Func<ICollection<int>> GetMoveDownByOptions;       

        public void DoLayout()
        {
            var attribute = Filter.GetType().GetCustomAttribute<AssetFilterAttribute>();
            var filterTitle = attribute == null
                ? ObjectNames.NicifyVariableName(Filter.GetType().Name)
                : attribute.DisplayName;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            // Click
            if (Event.current.type == EventType.MouseDown)
                _mouseButtonClickedSubject.OnNext(Empty.Default);

            // Title
            var titleRect = GUILayoutUtility.GetRect(1, 16, GUILayout.ExpandWidth(true));
            EditorGUI.LabelField(titleRect, filterTitle, EditorStyles.boldLabel);

            // Drawer
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(12);
                using (new EditorGUILayout.VerticalScope())
                {
                    using (var ccs = new EditorGUI.ChangeCheckScope())
                    {
                        _drawer.DoLayout();

                        // Notify when any value is changed.
                        if (ccs.changed)
                            _valueChangedSubject.OnNext(Empty.Default);
                    }
                }
            }

            // Right Click Menu
            if (Event.current.type == EventType.MouseDown
                && Event.current.button == 1
                && titleRect.Contains(Event.current.mousePosition))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(RemoveMenuName), false,
                    () => _removeMenuExecutedSubject.OnNext(Empty.Default));
                menu.AddItem(new GUIContent(MoveUpMenuName), false,
                    () => _moveUpMenuExecutedSubject.OnNext(Empty.Default));

                var moveUpByList = GetMoveUpByOptions?.Invoke();
                if (moveUpByList == null || moveUpByList.Count == 0)
                    menu.AddDisabledItem(new GUIContent(MoveUpByMenuName), false);
                else
                    foreach (var count in moveUpByList)
                        menu.AddItem(new GUIContent($"{MoveUpByMenuName}/{count}"), false,
                                     () => _moveUpByMenuExecutedSubject.OnNext(count));

                menu.AddItem(new GUIContent(MoveDownMenuName), false,
                    () => _moveDownMenuExecutedSubject.OnNext(Empty.Default));

                var moveDownByList = GetMoveDownByOptions?.Invoke();
                if (moveDownByList == null || moveDownByList.Count == 0)
                    menu.AddDisabledItem(new GUIContent(MoveDownByMenuName), false);
                else
                    foreach (var count in moveDownByList)
                        menu.AddItem(new GUIContent($"{MoveDownByMenuName}/{count}"), false,
                                     () => _moveDownByMenuExecutedSubject.OnNext(count));

                menu.AddItem(new GUIContent(CopyMenuName), false,
                    () => _copyMenuExecutedSubject.OnNext(Empty.Default));

                // Paste
                if (CanPaste == null || CanPaste.Invoke())
                    menu.AddItem(new GUIContent(PasteMenuName), false,
                        () => _pasteMenuExecutedSubject.OnNext(Empty.Default));
                else
                    menu.AddDisabledItem(new GUIContent(PasteMenuName), false);

                // Paste Values
                if (CanPasteValues == null || CanPasteValues.Invoke())
                    menu.AddItem(new GUIContent(PasteValuesMenuName), false,
                        () => _pasteValuesMenuExecutedSubject.OnNext(Empty.Default));
                else
                    menu.AddDisabledItem(new GUIContent(PasteValuesMenuName), false);

                menu.ShowAsContext();
            }

            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }
    }
}
