// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError
{
    [Serializable]
    public sealed class AddressRuleValidationError
    {
        [SerializeField] private string addressRuleId;
        [SerializeField] private string addressableAssetGroupGuid;
        [SerializeField] private string addressableAssetGroupName;
        [SerializeField] private AssetGroupValidationError[] assetGroupErrors;

        public AddressRuleValidationError(AddressRule addressRule, AssetGroupValidationError[] assetGroupErrors)
        {
            addressRuleId = addressRule.Id;
            addressableAssetGroupGuid = addressRule.AddressableGroup.Guid;
            addressableAssetGroupName = addressRule.AddressableGroup.Name;
            this.assetGroupErrors = assetGroupErrors;
        }

        public string AddressRuleId => addressRuleId;
        public string AddressableAssetGroupGuid => addressableAssetGroupGuid;
        public string AddressableAssetGroupName => addressableAssetGroupName;
        public IReadOnlyList<AssetGroupValidationError> AssetGroupErrors => assetGroupErrors;
    }
}
