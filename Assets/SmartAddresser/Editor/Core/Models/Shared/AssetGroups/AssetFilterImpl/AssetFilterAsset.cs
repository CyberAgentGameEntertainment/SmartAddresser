using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    public abstract class AssetFilterAsset : ScriptableObject, IAssetFilter
    {
        string IAssetFilter.Id => throw new InvalidOperationException();

        /// <inheritdoc />
        public abstract void SetupForMatching();

        /// <inheritdoc />
        public virtual bool Validate(out string errorMessage)
        {
            errorMessage = null;
            return true;
        }
        
        /// <inheritdoc />
        public abstract bool IsMatch(string assetPath, Type assetType, bool isFolder);

        /// <inheritdoc />
        public abstract string GetDescription();

        public virtual void OverwriteValuesFromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}
