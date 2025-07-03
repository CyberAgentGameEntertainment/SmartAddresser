using System;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups
{
    /// <summary>
    ///     Attribute to restrict AssetFilter usage to specific rule types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RestrictedToRulesAttribute : Attribute
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="allowedRuleTypes">The rule types that are allowed to use this filter.</param>
        public RestrictedToRulesAttribute(params RuleType[] allowedRuleTypes)
        {
            AllowedRuleTypes = allowedRuleTypes;
        }

        /// <summary>
        ///     The rule types that are allowed to use this filter.
        /// </summary>
        public RuleType[] AllowedRuleTypes { get; }
    }

    /// <summary>
    ///     Enum representing the different rule types.
    /// </summary>
    public enum RuleType
    {
        /// <summary>
        ///     Address rule type.
        /// </summary>
        Address,

        /// <summary>
        ///     Label rule type.
        /// </summary>
        Label,

        /// <summary>
        ///     Version rule type.
        /// </summary>
        Version
    }
}