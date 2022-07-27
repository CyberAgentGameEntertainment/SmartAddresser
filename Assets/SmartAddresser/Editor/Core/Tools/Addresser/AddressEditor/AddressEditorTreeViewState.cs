// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.AddressEditor
{
    [Serializable]
    internal sealed class AddressEditorTreeViewState : TreeViewState
    {
        [SerializeField] private MultiColumnHeaderState.Column[] _columnStates;

        public MultiColumnHeaderState.Column[] ColumnStates => _columnStates;

        public AddressEditorTreeViewState()
        {
            _columnStates = GetColumnStates();
        }

        private MultiColumnHeaderState.Column[] GetColumnStates()
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
                allowToggleVisibility = true
            };
            var addressRuleColumn = new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Address Rule"),
                headerTextAlignment = TextAlignment.Center,
                canSort = false,
                width = 200,
                minWidth = 50,
                autoResize = true,
                allowToggleVisibility = true
            };
            return new[] { groupsColumn, controlColumn, assetGroupsColumn, addressRuleColumn };
        }
    }
}
