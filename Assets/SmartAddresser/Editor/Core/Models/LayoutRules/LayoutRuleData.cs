using System.Collections.Generic;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    [CreateAssetMenu(fileName = "LayoutRuleData", menuName = "Smart Addresser/Layout Rule Data")]
    public sealed class LayoutRuleData : BaseLayoutRuleData
    {
        [SerializeField] private LayoutRule _layoutRule = new LayoutRule();

        public LayoutRule LayoutRule
        {
            get => _layoutRule;
            set => _layoutRule = value;
        }

        public override IEnumerable<LayoutRule> LayoutRules
        {
            get { yield return _layoutRule; }
        }
    }
}
