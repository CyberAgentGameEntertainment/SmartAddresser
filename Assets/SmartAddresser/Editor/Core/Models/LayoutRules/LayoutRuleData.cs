using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    [CreateAssetMenu(fileName = "LayoutRuleData", menuName = "Smart Addresser/Layout Rule Data")]
    public sealed class LayoutRuleData : ScriptableObject
    {
        [SerializeField] private LayoutRule _layoutRule = new LayoutRule();

        public LayoutRule LayoutRule
        {
            get => _layoutRule;
            set => _layoutRule = value;
        }
    }
}
