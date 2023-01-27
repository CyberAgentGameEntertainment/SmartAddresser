using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using UnityEditor;
using UnityEditor.Callbacks;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    public static class LayoutRuleDataOpener
    {
        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID);

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
