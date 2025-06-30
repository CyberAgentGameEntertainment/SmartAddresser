// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    internal sealed class ObjectBasedAssetFilterTest
    {
        [Test]
        public void IsMatch_RegisterMatchedObject_ReturnTrue()
        {
            var filter = new ObjectBasedAssetFilter();
            filter.Object.Value = AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.Texture64);
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture64, typeof(Texture2D), false, null, null), Is.True);
        }

        [TestCase(FolderTargetingMode.IncludedNonFolderAssets, TestAssetRelativePaths.Shared.Texture64, typeof(Texture2D),
            ExpectedResult = true)]
        [TestCase(FolderTargetingMode.IncludedNonFolderAssets, TestAssetRelativePaths.Shared.Folder, typeof(DefaultAsset),
            ExpectedResult = false)]
        [TestCase(FolderTargetingMode.Self, TestAssetRelativePaths.Shared.Texture64, typeof(Texture2D), ExpectedResult = false)]
        [TestCase(FolderTargetingMode.Self, TestAssetRelativePaths.Shared.Folder, typeof(DefaultAsset), ExpectedResult = true)]
        [TestCase(FolderTargetingMode.Both, TestAssetRelativePaths.Shared.Texture64, typeof(Texture2D), ExpectedResult = true)]
        [TestCase(FolderTargetingMode.Both, TestAssetRelativePaths.Shared.Folder, typeof(DefaultAsset), ExpectedResult = true)]
        public bool IsMatch_ObjectIsFolder(FolderTargetingMode targetingMode, string relativeAssetPath, Type assetType)
        {
            var filter = new ObjectBasedAssetFilter();
            filter.FolderTargetingMode = targetingMode;
            filter.Object.Value = AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.Folder);
            filter.SetupForMatching();
            var assetPath = TestAssetPaths.CreateAbsoluteAssetPath(relativeAssetPath);
            return filter.IsMatch(assetPath, assetType, assetType == typeof(DefaultAsset), null, null);
        }

        [TestCase(FolderTargetingMode.IncludedNonFolderAssets, TestAssetRelativePaths.Dummy1.Folder, typeof(DefaultAsset), ExpectedResult = false)]
        [TestCase(FolderTargetingMode.IncludedNonFolderAssets, TestAssetRelativePaths.Dummy1.PrefabDummy, typeof(GameObject), ExpectedResult = false)]
        [TestCase(FolderTargetingMode.IncludedNonFolderAssets, TestAssetRelativePaths.PrefabDummy, typeof(GameObject), ExpectedResult = false)]
        [TestCase(FolderTargetingMode.Self, TestAssetRelativePaths.Dummy1.Folder, typeof(DefaultAsset), ExpectedResult = false)]
        [TestCase(FolderTargetingMode.Self, TestAssetRelativePaths.Dummy1.PrefabDummy, typeof(GameObject), ExpectedResult = false)]
        [TestCase(FolderTargetingMode.Self, TestAssetRelativePaths.PrefabDummy, typeof(GameObject), ExpectedResult = false)]
        [TestCase(FolderTargetingMode.Both, TestAssetRelativePaths.Dummy1.Folder, typeof(DefaultAsset), ExpectedResult = false)]
        [TestCase(FolderTargetingMode.Both, TestAssetRelativePaths.Dummy1.PrefabDummy, typeof(GameObject), ExpectedResult = false)]
        [TestCase(FolderTargetingMode.Both, TestAssetRelativePaths.PrefabDummy, typeof(GameObject), ExpectedResult = false)]
        public bool IsMatch_ObjectIsFolder_ContainsFilterDirectoryName_ReturnFalse(FolderTargetingMode targetingMode, string relativeAssetPath, Type assetType)
        {
            var filter = new ObjectBasedAssetFilter();
            filter.FolderTargetingMode = targetingMode;
            filter.Object.Value = AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Dummy.Folder);
            filter.SetupForMatching();
            var assetPath = TestAssetPaths.CreateAbsoluteAssetPath(relativeAssetPath);
            return filter.IsMatch(assetPath, assetType, assetType == typeof(DefaultAsset), null, null);
        }

        [Test]
        public void IsMatch_RegisterNotMatchedObject_ReturnFalse()
        {
            var filter = new ObjectBasedAssetFilter();
            filter.Object.Value = AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.Texture64);
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture128, typeof(Texture2D), false, null, null), Is.False);
        }

        [Test]
        public void IsMatch_RegisterObjectsAndContainsMatched_ReturnTrue()
        {
            var filter = new ObjectBasedAssetFilter();
            filter.Object.IsListMode = true;
            filter.Object.AddValue(AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.Texture64));
            filter.Object.AddValue(AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.Texture128));
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture64, typeof(Texture2D), false, null, null), Is.True);
        }

        [Test]
        public void IsMatch_RegisterObjectsAndNotContainsMatched_ReturnFalse()
        {
            var filter = new ObjectBasedAssetFilter();
            filter.Object.IsListMode = true;
            filter.Object.AddValue(AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.Texture128));
            filter.Object.AddValue(AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.Texture256));
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture64, typeof(Texture2D), false, null, null), Is.False);
        }
        
        [Test]
        public void IsMatch_AnotherDirectoryContainsFilterDirectoryName_ReturnFalse()
        {
            var filter = new ObjectBasedAssetFilter();
            filter.Object.IsListMode = true;
            filter.Object.AddValue(AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Dummy.Folder));
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Dummy1.PrefabDummy, typeof(GameObject), false, null, null), Is.False);
        }

        [Test]
        public void Validate_ObjectIsNotNull_ReturnTrue()
        {
            var filter = new ObjectBasedAssetFilter();
            filter.Object.Value = AssetDatabase.LoadAssetAtPath<Object>(TestAssetPaths.Shared.Texture64);
            filter.SetupForMatching();
            
            Assert.That(filter.Validate(out _), Is.True);
        }
        
        [Test]
        public void Validate_ObjectIsNull_ReturnFalse()
        {
            var filter = new ObjectBasedAssetFilter();
            filter.SetupForMatching();
            
            Assert.That(filter.Validate(out _), Is.False);
        }
    }
}
