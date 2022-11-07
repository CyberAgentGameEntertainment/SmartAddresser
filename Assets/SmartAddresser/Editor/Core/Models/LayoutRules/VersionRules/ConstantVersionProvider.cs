using System;
using SmartAddresser.Editor.Core.Models.Shared;
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

        void IProvider<string>.Setup()
        {
        }

        string IProvider<string>.Provide(string assetPath, Type assetType, bool isFolder)
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
