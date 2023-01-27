using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;

namespace Development.Editor.Core.Tools.Addresser.Shared
{
    internal sealed class FakeLayoutRuleDataRepository : ILayoutRuleDataRepository
    {
        private readonly ObservableProperty<LayoutRuleData> _editingData = new ObservableProperty<LayoutRuleData>();
        private readonly List<LayoutRuleData> _dataList = new List<LayoutRuleData>();

        public IReadOnlyObservableProperty<LayoutRuleData> EditingData=> _editingData;

        public IReadOnlyList<LayoutRuleData> LoadAll()
        {
            return _dataList;
        }

        public void SetEditingData(LayoutRuleData data)
        {
            _editingData.Value = data;
        }

        public void SetEditingDataAndNotNotify(LayoutRuleData data)
        {
            _editingData.SetValueAndNotNotify(data);
        }

        public void AddData(LayoutRuleData data)
        {
            if (_dataList.Count == 0)
                SetEditingData(data);
            _dataList.Add(data);
        }
    }
}
