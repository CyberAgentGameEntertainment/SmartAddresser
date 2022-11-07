using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    public interface ILayoutRuleDataRepository
    {
        IReadOnlyObservableProperty<LayoutRuleData> ActiveData { get; }

        IReadOnlyList<LayoutRuleData> LoadAll();

        void SetActiveData(LayoutRuleData data);

        void SetActiveDataAndNotNotify(LayoutRuleData data);
    }
}
