using System;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    internal sealed class DependentObjectBasedAssetFilterTest
    {
        [TestCase(TestAssetRelativePaths.Shared.Texture64, typeof(Texture2D), ExpectedResult = true)]
        [TestCase(TestAssetRelativePaths.Shared.Texture128, typeof(Texture2D), ExpectedResult = false)]
        [TestCase(TestAssetRelativePaths.Shared.PrefabTex64, typeof(GameObject), ExpectedResult = true)]
        public bool IsMatch(string relativeAssetPath, Type assetType)
        {
            var filter = new DependentObjectBasedAssetFilter();
            filter.Object.Value = AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.PrefabTex64);
            filter.SetupForMatching();
            var assetPath = TestAssetPaths.CreateAbsoluteAssetPath(relativeAssetPath);
            return filter.IsMatch(assetPath, assetType, assetType == typeof(DefaultAsset), null, null);
        }

        [TestCase(TestAssetRelativePaths.Shared.Texture64, typeof(Texture2D), false, ExpectedResult = true)]
        [TestCase(TestAssetRelativePaths.Shared.Texture64, typeof(Texture2D), true, ExpectedResult = false)]
        [TestCase(TestAssetRelativePaths.Shared.MaterialTex64, typeof(Material), false, ExpectedResult = true)]
        [TestCase(TestAssetRelativePaths.Shared.MaterialTex64, typeof(Material), true, ExpectedResult = true)]
        public bool IsMatch_OnlyDirectDependencies(
            string relativeAssetPath,
            Type assetType,
            bool onlyDirectDependencies
        )
        {
            var filter = new DependentObjectBasedAssetFilter();
            filter.Object.Value = AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.PrefabTex64);
            filter.OnlyDirectDependencies = onlyDirectDependencies;
            filter.SetupForMatching();
            var assetPath = TestAssetPaths.CreateAbsoluteAssetPath(relativeAssetPath);
            return filter.IsMatch(assetPath, assetType, assetType == typeof(DefaultAsset), null, null);
        }

        [Test]
        public void Validate_ObjectIsNotNull_ReturnTrue()
        {
            var filter = new DependentObjectBasedAssetFilter();
            filter.Object.Value = AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.Texture64);
            filter.SetupForMatching();

            Assert.That(filter.Validate(out _), Is.True);
        }

        [Test]
        public void Validate_ObjectIsNull_ReturnFalse()
        {
            var filter = new DependentObjectBasedAssetFilter();
            filter.SetupForMatching();

            Assert.That(filter.Validate(out _), Is.False);
        }
    }
}
