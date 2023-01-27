using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    public interface ILayoutRuleDataRepository
    {
        IReadOnlyObservableProperty<LayoutRuleData> EditingData { get; }

        IReadOnlyList<LayoutRuleData> LoadAll();

        void SetEditingData(LayoutRuleData data);

        void SetEditingDataAndNotNotify(LayoutRuleData data);
    }
}
