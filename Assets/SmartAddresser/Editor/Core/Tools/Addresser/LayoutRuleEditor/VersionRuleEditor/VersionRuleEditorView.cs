using System;
using SmartAddresser.Editor.Foundation.EditorSplitView;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor
{
    /// <summary>
    ///     View for the version rule editor.
    /// </summary>
    internal sealed class VersionRuleEditorView : IDisposable
    {
        private readonly Action _repaintParentWindow;
        private readonly EditorGUILayoutSplitView _splitView;

        public VersionRuleEditorView(VersionRuleListTreeView.State treeViewState,
            EditorGUILayoutSplitViewState splitViewState, Action repaintParentWindow)
        {
            _splitView = new EditorGUILayoutSplitView(splitViewState);
            _repaintParentWindow = repaintParentWindow;
            ListView = new VersionRuleListView(treeViewState);
            InspectorView = new VersionRuleEditorInspectorView();
        }

        public VersionRuleListView ListView { get; }

        public VersionRuleEditorInspectorView InspectorView { get; }

        public void Dispose()
        {
            InspectorView.Dispose();
        }

        public void DoLayout()
        {
            _splitView.Begin();

            ListView.DoLayout();

            if (_splitView.Split())
                _repaintParentWindow();

            InspectorView.DoLayout();

            _splitView.End();
        }
    }
}
