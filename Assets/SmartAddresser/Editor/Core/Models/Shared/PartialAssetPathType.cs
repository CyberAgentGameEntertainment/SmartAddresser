using System;
using System.IO;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    public enum PartialAssetPathType
    {
        FileName,
        FileNameWithoutExtensions,
        AssetPath
    }

    public static class PartialAssetPathTypeExtensions
    {
        public static string Create(this PartialAssetPathType self, string assetPath)
        {
            Assert.IsFalse(string.IsNullOrEmpty(assetPath));

            return self switch
            {
                PartialAssetPathType.FileName => Path.GetFileName(assetPath),
                PartialAssetPathType.FileNameWithoutExtensions => Path.GetFileNameWithoutExtension(assetPath),
                PartialAssetPathType.AssetPath => assetPath,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
