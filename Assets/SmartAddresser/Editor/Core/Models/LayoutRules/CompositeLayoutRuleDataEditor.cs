using SmartAddresser.Editor.Core.Tools.Shared;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    [CustomEditor(typeof(CompositeLayoutRuleData))]
    internal sealed class CompositeLayoutRuleDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var data = (CompositeLayoutRuleData)target;

            if (GUILayout.Button("Apply"))
                MenuActions.ApplyAction(data);

            base.OnInspectorGUI();
        }
    }
}
