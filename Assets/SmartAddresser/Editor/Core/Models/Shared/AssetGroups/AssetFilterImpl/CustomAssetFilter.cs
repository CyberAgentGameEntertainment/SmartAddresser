using System;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    [Serializable]
    public sealed class CustomAssetFilter : IAssetFilter
    {
        [SerializeField] private string id;
        [SerializeField] private AssetFilterAsset assetFilter;

        public CustomAssetFilter()
        {
            id = IdentifierFactory.Create();
        }

        public AssetFilterAsset AssetFilter
        {
            get => assetFilter;
            set => assetFilter = value;
        }

        public string Id => id;

        public void SetupForMatching()
        {
            if (assetFilter != null)
                assetFilter.SetupForMatching();
        }

        public bool Validate(out AssetFilterValidationError error)
        {
            if (assetFilter == null)
            {
                error = new AssetFilterValidationError(this, new[] { "AssetFilter is null." });
                return false;
            }

            return assetFilter.Validate(out error);
        }

        public bool IsMatch(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup)
        {
            if (assetFilter == null)
                return false;

            return assetFilter.IsMatch(assetPath, assetType, isFolder, address, addressableAssetGroup);
        }

        public string GetDescription()
        {
            if (assetFilter == null)
                return string.Empty;

            return assetFilter.GetDescription();
        }

        public void OverwriteValuesFromJson(string json)
        {
            var idCache = id;
            assetFilter.OverwriteValuesFromJson(json);
            id = idCache;
        }
    }
}
