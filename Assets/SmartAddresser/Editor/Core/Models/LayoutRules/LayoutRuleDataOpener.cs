using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_6000_2_OR_NEWER
using UnityEngine;
#endif

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    public static class LayoutRuleDataOpener
    {
        [OnOpenAsset(0)]
#if UNITY_6000_2_OR_NEWER
        public static bool OnOpen(EntityId entityId, int line)
        {
            var asset = EditorUtility.EntityIdToObject(entityId);
#else
        public static bool OnOpen(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID);
#endif

            if (asset is LayoutRuleData data)
            {
                var repository = new LayoutRuleDataRepository();
                repository.SetEditingData(data);
                LayoutRuleEditorWindow.Open();
                return true;
            }

            return false;
        }
    }
}
