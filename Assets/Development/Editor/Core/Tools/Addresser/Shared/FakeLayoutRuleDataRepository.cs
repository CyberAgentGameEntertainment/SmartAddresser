using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace Development.Editor.Core.Tools.Addresser.Shared
{
    internal sealed class FakeLayoutRuleDataRepository : ILayoutRuleDataRepository
    {
        private readonly ObservableProperty<LayoutRuleData> _activeData = new ObservableProperty<LayoutRuleData>();
        private readonly List<LayoutRuleData> _dataList = new List<LayoutRuleData>();

        public IReadOnlyObservableProperty<LayoutRuleData> ActiveData => _activeData;

        public IReadOnlyList<LayoutRuleData> LoadAll()
        {
            return _dataList;
        }

        public void SetActiveData(LayoutRuleData data)
        {
            _activeData.Value = data;
        }

        public void SetActiveDataAndNotNotify(LayoutRuleData data)
        {
            _activeData.SetValueAndNotNotify(data);
        }

        public void AddData(LayoutRuleData data)
        {
            if (_dataList.Count == 0)
                SetActiveData(data);
            _dataList.Add(data);
        }
    }
}
