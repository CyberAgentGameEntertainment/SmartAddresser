using SmartAddresser.Editor.Core.Models.EntryRules.AddressRules;
using UnityEditor.IMGUI.Controls;

namespace SmartAddresser.Editor.Core.Tools.Addresser.AddressEditor
{
    internal sealed class AddressRuleEditorTreeViewItem : TreeViewItem
    {
        public AddressRuleEditorTreeViewItem(AddressRule rule)
        {
            Rule = rule;
        }

        public AddressRule Rule { get; }
    }
}
