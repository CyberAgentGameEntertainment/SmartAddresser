using System;
using SmartAddresser.Editor.Foundation.EditorSplitView;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor
{
    /// <summary>
    ///     View for the label rule editor.
    /// </summary>
    internal sealed class LabelRuleEditorView : IDisposable
    {
        private readonly Action _repaintParentWindow;
        private readonly EditorGUILayoutSplitView _splitView;

        public LabelRuleEditorView(LabelRuleListTreeView.State treeViewState,
            EditorGUILayoutSplitViewState splitViewState, Action repaintParentWindow)
        {
            _splitView = new EditorGUILayoutSplitView(splitViewState);
            _repaintParentWindow = repaintParentWindow;
            ListView = new LabelRuleListView(treeViewState);
            InspectorView = new LabelRuleEditorInspectorView();
        }

        public LabelRuleListView ListView { get; }

        public LabelRuleEditorInspectorView InspectorView { get; }

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
