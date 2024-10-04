// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError
{
    [Serializable]
    public sealed class VersionRuleValidationError
    {
        [SerializeField] private string versionRuleId;
        [SerializeField] private string versionRuleName;
        [SerializeField] private AssetGroupValidationError[] assetGroupErrors;

        public VersionRuleValidationError(VersionRule versionRule, AssetGroupValidationError[] assetGroupErrors)
        {
            versionRuleId = versionRule.Id;
            versionRuleName = versionRule.Name.Value;
            this.assetGroupErrors = assetGroupErrors;
        }

        public string VersionRuleId => versionRuleId;
        public string VersionRuleName => versionRuleName;
        public IReadOnlyList<AssetGroupValidationError> AssetGroupErrors => assetGroupErrors;
    }
}
