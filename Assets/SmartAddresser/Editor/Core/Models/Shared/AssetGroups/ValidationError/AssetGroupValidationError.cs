// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError
{
    [Serializable]
    public sealed class AssetGroupValidationError
    {
        [SerializeField] private string assetGroupId;
        [SerializeField] private string assetGroupName;
        [SerializeField] private AssetFilterValidationError[] assetFilterErrors;

        public AssetGroupValidationError(
            AssetGroup assetGroup,
            AssetFilterValidationError[] assetFilterErrors
        )
        {
            assetGroupId = assetGroup.Id;
            assetGroupName = assetGroup.Name.Value;
            this.assetFilterErrors = assetFilterErrors;
        }

        public string AssetGroupId => assetGroupId;
        public string AssetGroupName => assetGroupName;
        public IReadOnlyList<AssetFilterValidationError> AssetFilterErrors => assetFilterErrors;
    }
}
