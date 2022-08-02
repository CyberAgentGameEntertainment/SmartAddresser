using System;
using System.IO;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    public enum PartialAssetPathType
    {
        AssetName,
        AssetNameWithoutExtensions,
        AssetPath
    }

    public static class PartialAssetPathTypeExtensions
    {
        public static string Create(this PartialAssetPathType self, string assetPath)
        {
            Assert.IsFalse(string.IsNullOrEmpty(assetPath));

            return self switch
            {
                PartialAssetPathType.AssetName => Path.GetFileName(assetPath),
                PartialAssetPathType.AssetNameWithoutExtensions => Path.GetFileNameWithoutExtension(assetPath),
                PartialAssetPathType.AssetPath => assetPath,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
