using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    [CustomEditor(typeof(LayoutRuleData))]
    internal sealed class LayoutRuleDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var data = (LayoutRuleData)target;

            if (GUILayout.Button("Open Editor"))
            {
                var repository = new LayoutRuleDataRepository();
                repository.SetEditingData(data);
                LayoutRuleEditorWindow.Open();
            }

            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}
