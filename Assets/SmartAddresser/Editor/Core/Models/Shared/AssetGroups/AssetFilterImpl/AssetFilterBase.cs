using System;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    [Serializable]
    public abstract class AssetFilterBase : IAssetFilter
    {
        [SerializeField] private string _id;

        protected AssetFilterBase()
        {
            _id = IdentifierFactory.Create();
        }

        public string Id => _id;

        /// <inheritdoc />
        public abstract void SetupForMatching();

        /// <inheritdoc />
        public abstract bool Validate(out AssetFilterValidationError error);

        /// <inheritdoc />
        public abstract bool IsMatch(string assetPath, Type assetType, bool isFolder, string address,
            AddressableAssetGroup addressableAssetGroup);

        /// <inheritdoc />
        public abstract string GetDescription();

        public void OverwriteValuesFromJson(string json)
        {
            var id = _id;
            JsonUtility.FromJsonOverwrite(json, this);
            _id = id;
        }
    }
}
