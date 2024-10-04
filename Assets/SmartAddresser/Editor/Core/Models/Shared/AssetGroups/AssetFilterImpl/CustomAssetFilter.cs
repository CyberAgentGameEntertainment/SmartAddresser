using System;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
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
            assetFilter.SetupForMatching();
        }

        public bool Validate(out AssetFilterValidationError error)
        {
            return assetFilter.Validate(out error);
        }

        public bool IsMatch(string assetPath, Type assetType, bool isFolder)
        {
            return assetFilter.IsMatch(assetPath, assetType, isFolder);
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
