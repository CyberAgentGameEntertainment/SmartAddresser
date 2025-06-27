using System;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    /// <summary>
    ///     Provide constant tag.
    /// </summary>
    [Serializable]
    public sealed class ConstantVersionProvider : IVersionProvider
    {
        [SerializeField] private string _version;

        public string Version
        {
            get => _version;
            set => _version = value;
        }

        public void Setup()
        {
        }

        public string Provide(string assetPath, Type assetType, bool isFolder, string address, AddressableAssetGroup addressableAssetGroup)
        {
            if (string.IsNullOrEmpty(_version))
                return null;

            return _version;
        }

        public string GetDescription()
        {
            if (string.IsNullOrEmpty(_version))
                return null;

            return $"Constant: {_version}";
        }
    }
}
