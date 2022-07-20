using System;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups
{
    [Serializable]
    public sealed class ObservableAssetGroupDictionary : ObservableDictionary<string, AssetGroup>
    {
    }
}
