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
        public IReadOnlyObservableProperty<LayoutRuleData> ActiveData
            => SmartAddresserPreferences.instance.ActiveLayoutRuleData;

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

        public void SetActiveData(LayoutRuleData data)
        {
            SmartAddresserPreferences.instance.SetActiveLayoutRuleData(data);
        }

        public void SetActiveDataAndNotNotify(LayoutRuleData data)
        {
            SmartAddresserPreferences.instance.SetActiveLayoutRuleDataAndNotNotify(data);
        }
    }
}
