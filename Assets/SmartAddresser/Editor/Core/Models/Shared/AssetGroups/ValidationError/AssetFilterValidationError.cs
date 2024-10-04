// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError
{
    [Serializable]
    public sealed class AssetFilterValidationError
    {
        [SerializeField] private string assetFilterId;
        [SerializeField] private string assetFilterTypeName;
        [SerializeField] private string[] errorMessages;

        public AssetFilterValidationError(IAssetFilter assetFilter, string[] errorMessages)
        {
            assetFilterId = assetFilter.Id;
            assetFilterTypeName = assetFilter.GetType().Name;
            this.errorMessages = errorMessages;
        }

        public string AssetFilterId => assetFilterId;
        public string AssetFilterTypeName => assetFilterTypeName;
        public IReadOnlyList<string> ErrorMessages => errorMessages;
    }
}
