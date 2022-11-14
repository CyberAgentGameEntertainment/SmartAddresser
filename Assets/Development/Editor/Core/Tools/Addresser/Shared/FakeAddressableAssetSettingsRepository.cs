using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using UnityEditor.AddressableAssets.Settings;

namespace Development.Editor.Core.Tools.Addresser.Shared
{
    internal sealed class FakeAddressableAssetSettingsRepository : IAddressableAssetSettingsRepository
    {
        public Dictionary<LayoutRuleData, AddressableAssetSettings> DataSettingsMap =
            new Dictionary<LayoutRuleData, AddressableAssetSettings>();

        public AddressableAssetSettings Get(LayoutRuleData layoutRuleData)
        {
            return DataSettingsMap[layoutRuleData];
        }
    }
}
