using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    [CreateAssetMenu(fileName = "CompositeLayoutRuleData", menuName = "Smart Addresser/Composite Layout Rule Data")]
    public sealed class CompositeLayoutRuleData : BaseLayoutRuleData
    {
        [SerializeField] private LayoutRuleData[] _layoutRules;

        public override IEnumerable<LayoutRule> LayoutRules => _layoutRules.SelectMany(x => x.LayoutRules);
    }
}
