// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError
{
    [Serializable]
    public sealed class LayoutRuleValidationError
    {
        [SerializeField] private AddressRuleValidationError[] addressRuleErrors;
        [SerializeField] private LabelRuleValidationError[] labelRuleErrors;
        [SerializeField] private VersionRuleValidationError[] versionRuleErrors;

        public LayoutRuleValidationError(
            AddressRuleValidationError[] addressRuleErrors,
            LabelRuleValidationError[] labelRuleErrors,
            VersionRuleValidationError[] versionRuleErrors
        )
        {
            this.addressRuleErrors = addressRuleErrors;
            this.labelRuleErrors = labelRuleErrors;
            this.versionRuleErrors = versionRuleErrors;
        }

        public IReadOnlyList<AddressRuleValidationError> AddressRuleErrors => addressRuleErrors;
        public IReadOnlyList<LabelRuleValidationError> LabelRuleErrors => labelRuleErrors;
        public IReadOnlyList<VersionRuleValidationError> VersionRuleErrors => versionRuleErrors;

        public string ToJson(bool prettyPrint)
        {
            return JsonUtility.ToJson(this, prettyPrint);
        }
    }
}
