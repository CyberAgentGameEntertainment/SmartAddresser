using SmartAddresser.Editor.Core.Models.LayoutRules;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    public sealed class AddressableAssetSettingsRepository : IAddressableAssetSettingsRepository
    {
        public AddressableAssetSettings Get(LayoutRuleData layoutRuleData)
        {
            return AddressableAssetSettingsDefaultObject.Settings;
        }
    }
}
