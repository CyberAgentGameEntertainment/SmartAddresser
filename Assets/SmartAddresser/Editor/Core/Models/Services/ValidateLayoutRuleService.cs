// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;

namespace SmartAddresser.Editor.Core.Models.Services
{
    public sealed class ValidateLayoutRuleService
    {
        private readonly LayoutRule _layoutRule;

        public ValidateLayoutRuleService(LayoutRule layoutRule)
        {
            _layoutRule = layoutRule;
        }

        public bool Execute(bool doSetup, out LayoutRuleValidationError error)
        {
            if (doSetup)
                _layoutRule.Setup();

            return _layoutRule.Validate(out error);
        }
    }
}
