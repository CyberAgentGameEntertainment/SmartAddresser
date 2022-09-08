using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    public sealed class LayoutRuleData : ScriptableObject
    {
        [SerializeField] private LayoutRule _layoutRule;

        public LayoutRule LayoutRule => _layoutRule;
    }
}
