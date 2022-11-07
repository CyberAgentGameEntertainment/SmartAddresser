using SmartAddresser.Editor.Core.Models.LayoutRules;
using UnityEditor.AddressableAssets.Settings;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    public interface IAddressableAssetSettingsRepository
    {
        AddressableAssetSettings Get(LayoutRuleData layoutRuleData);
    }
}
