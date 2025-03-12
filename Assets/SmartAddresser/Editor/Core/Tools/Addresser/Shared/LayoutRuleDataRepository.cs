using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    public sealed class LayoutRuleDataRepository : ILayoutRuleDataRepository
    {
        public BaseLayoutRuleData PrimaryData => SmartAddresserProjectSettings.instance.PrimaryData;

        public IReadOnlyObservableProperty<LayoutRuleData> EditingData =>
            SmartAddresserPreferences.instance.EditingData;

        public IReadOnlyList<LayoutRuleData> LoadAll()
        {
            return AssetDatabase.FindAssets($"t:{nameof(LayoutRuleData)}")
                .Select(x =>
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(x);
                    return AssetDatabase.LoadAssetAtPath<LayoutRuleData>(assetPath);
                })
                .ToArray();
        }

        public void SetEditingData(LayoutRuleData data)
        {
            SmartAddresserPreferences.instance.SetEditingData(data);
        }

        public void SetEditingDataAndNotNotify(LayoutRuleData data)
        {
            SmartAddresserPreferences.instance.SetEditingDataAndNotNotify(data);
        }
    }
}
