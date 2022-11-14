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
        private ObservableProperty<LayoutRuleData> _activeLayoutRuleData = new ObservableProperty<LayoutRuleData>();

        public IReadOnlyObservableProperty<LayoutRuleData> ActiveLayoutRuleData => _activeLayoutRuleData;

        public void SetActiveLayoutRuleData(LayoutRuleData value)
        {
            if (value == _activeLayoutRuleData.Value)
                return;

            _activeLayoutRuleData.Value = value;
            Save(true);
        }

        public void SetActiveLayoutRuleDataAndNotNotify(LayoutRuleData value)
        {
            if (value == _activeLayoutRuleData.Value)
                return;

            _activeLayoutRuleData.SetValueAndNotNotify(value);
            Save(true);
        }
    }
}
