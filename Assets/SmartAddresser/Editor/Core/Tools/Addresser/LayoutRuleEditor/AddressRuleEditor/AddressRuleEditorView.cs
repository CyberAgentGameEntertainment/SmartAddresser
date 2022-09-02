using System;
using SmartAddresser.Editor.Foundation.EditorSplitView;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     View for the address editor.
    /// </summary>
    [Serializable]
    internal sealed class AddressRuleEditorView : IDisposable
    {
        private readonly Action _repaintParentWindow;
        private readonly EditorGUILayoutSplitView _splitView;

        public AddressRuleEditorView(AddressRuleListTreeView.State treeViewState,
            EditorGUILayoutSplitViewState splitViewState, Action repaintParentWindow)
        {
            _splitView = new EditorGUILayoutSplitView(splitViewState);
            _repaintParentWindow = repaintParentWindow;
            ListView = new AddressRuleEditorListView(treeViewState);
            InspectorView = new AddressRuleEditorInspectorView();
        }

        public AddressRuleEditorListView ListView { get; }

        public AddressRuleEditorInspectorView InspectorView { get; }

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
