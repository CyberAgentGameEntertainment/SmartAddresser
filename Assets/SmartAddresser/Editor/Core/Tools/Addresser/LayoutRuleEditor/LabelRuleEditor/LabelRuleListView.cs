using System;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation.EasyTreeView;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    /// <summary>
    ///     View for the left side of the label rule editor.
    /// </summary>
    internal sealed class LabelRuleListView
    {
        private readonly Subject<Empty> _addButtonClickedSubject = new Subject<Empty>();
        private readonly TreeViewSearchField _searchField;

        public LabelRuleListView(LabelRuleListTreeView.State treeViewState)
        {
            TreeView = new LabelRuleListTreeView(treeViewState);
            _searchField = new TreeViewSearchField(TreeView);
        }

        public IObservable<Empty> AddButtonClickedAsObservable => _addButtonClickedSubject;

        public LabelRuleListTreeView TreeView { get; }

        public void DoLayout()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // Plus Button
                var plusIconRect = GUILayoutUtility.GetRect(20, 20);
                var plusIconTexture = EditorGUIUtility.IconContent(EditorGUIUtil.ToolbarPlusIconName).image;
                GUI.DrawTexture(plusIconRect, plusIconTexture, ScaleMode.StretchToFill);
                if (GUI.Button(plusIconRect, "", GUIStyle.none))
                    _addButtonClickedSubject.OnNext(Empty.Default);

                // Search Field
                _searchField.OnToolbarGUI();

                GUILayout.FlexibleSpace();
            }

            // Tree View
            var treeViewRect =
                GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            TreeView.OnGUI(treeViewRect);
        }
    }
}
