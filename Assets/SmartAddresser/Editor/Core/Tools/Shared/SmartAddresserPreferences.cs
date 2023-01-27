using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    [FilePath("SmartAddresser/Preferences.asset", FilePathAttribute.Location.PreferencesFolder)]
    public sealed class SmartAddresserPreferences : ScriptableSingleton<SmartAddresserPreferences>
    {
        [SerializeField]
        private ObservableProperty<LayoutRuleData> editingData = new ObservableProperty<LayoutRuleData>();

        public IReadOnlyObservableProperty<LayoutRuleData> EditingData => editingData;

        public void SetEditingData(LayoutRuleData value)
        {
            if (value == editingData.Value)
                return;

            editingData.Value = value;
            Save(true);
        }

        public void SetEditingDataAndNotNotify(LayoutRuleData value)
        {
            if (value == editingData.Value)
                return;

            editingData.SetValueAndNotNotify(value);
            Save(true);
        }
    }
}
