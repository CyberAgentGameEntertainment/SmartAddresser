using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Foundation.ListableProperty
{
    public sealed class AddressableAssetGroupListablePropertyGUI : ListablePropertyGUI<AddressableAssetGroup>
    {
        public AddressableAssetGroupListablePropertyGUI(string displayName,
            ListableProperty<AddressableAssetGroup> list)
            : base(displayName, list, (rect, label, value, onValueChanged) =>
            {
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    var newValue = (AddressableAssetGroup)EditorGUI.ObjectField(rect, label, value,
                        typeof(AddressableAssetGroup), false);
                    if (ccs.changed)
                        onValueChanged.Invoke(newValue);
                }
            })
        {
        }
    }
}