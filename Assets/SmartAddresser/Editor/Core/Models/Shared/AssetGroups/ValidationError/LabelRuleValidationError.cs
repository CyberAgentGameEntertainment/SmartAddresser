// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError
{
    [Serializable]
    public sealed class LabelRuleValidationError
    {
        [SerializeField] private string labelRuleId;
        [SerializeField] private string labelRuleName;
        [SerializeField] private AssetGroupValidationError[] assetGroupErrors;

        public LabelRuleValidationError(LabelRule labelRule, AssetGroupValidationError[] assetGroupErrors)
        {
            labelRuleId = labelRule.Id;
            labelRuleName = labelRule.Name.Value;
            this.assetGroupErrors = assetGroupErrors;
        }

        public string LabelRuleId => labelRuleId;
        public string LabelRuleName => labelRuleName;
        public IReadOnlyList<AssetGroupValidationError> AssetGroupErrors => assetGroupErrors;
    }
}
