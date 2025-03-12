using System.Collections.Generic;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    public abstract class BaseLayoutRuleData : ScriptableObject
    {
        public abstract IEnumerable<LayoutRule> LayoutRules { get; }
    }
}
