// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    internal sealed class RegexBasedAssetFilterTest
    {
        [Test]
        public void IsMatch_RegisterMatchedRegex_ReturnTrue()
        {
            var filter = new RegexBasedAssetFilter();
            filter.AssetPathRegex.Value = "^Assets/Test/.+";
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test/Test.png", typeof(Texture2D), false, null, null), Is.True);
        }

        [Test]
        public void IsMatch_RegisterNotMatchedRegex_ReturnFalse()
        {
            var filter = new RegexBasedAssetFilter();
            filter.AssetPathRegex.Value = "^Assets/Test2/.+";
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test/Test.png", typeof(Texture2D), false, null, null), Is.False);
        }

        [Test]
        public void IsMatch_RegisterInvalidRegex_ReturnFalse()
        {
            var filter = new RegexBasedAssetFilter();
            filter.AssetPathRegex.Value = "^Assets/(Test/.+";
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test/Test.png", typeof(Texture2D), false, null, null), Is.False);
        }

        [TestCase(AssetFilterCondition.ContainsMatched, "^Assets/Test/.+", "^Assets/Test2/.+", ExpectedResult = true)]
        [TestCase(AssetFilterCondition.ContainsMatched, "^Assets/Test2/.+", "^Assets/Test3/.+", ExpectedResult = false)]
        [TestCase(AssetFilterCondition.MatchAll, "^Assets/Test/.+", ".+/Test/.+", ExpectedResult = true)]
        [TestCase(AssetFilterCondition.MatchAll, "^Assets/Test/.+", ".+/NotMatched/.+", ExpectedResult = false)]
        [TestCase(AssetFilterCondition.ContainsUnmatched, "^Assets/Test/.+", "^Assets/Test2/.+", ExpectedResult = true)]
        [TestCase(AssetFilterCondition.ContainsUnmatched,
            "^Assets/Test/.+",
            "^Assets/Test/Test.+",
            ExpectedResult = false)]
        [TestCase(AssetFilterCondition.NotMatchAll, "^Assets/Test2/.+", ".+/Test2/.+", ExpectedResult = true)]
        [TestCase(AssetFilterCondition.NotMatchAll, "^Assets/Test/.+", ".+/NotMatched/.+", ExpectedResult = false)]
        public bool IsMatch_MultipleAssetPaths(AssetFilterCondition condition, string assetPath1, string assetPath2)
        {
            var filter = new RegexBasedAssetFilter();
            filter.Condition = condition;
            filter.AssetPathRegex.IsListMode = true;
            filter.AssetPathRegex.AddValue(assetPath1);
            filter.AssetPathRegex.AddValue(assetPath2);
            filter.SetupForMatching();
            return filter.IsMatch("Assets/Test/Test.png", typeof(Texture2D), false, null, null);
        }

        [TestCase(true, "^Assets/Test", ExpectedResult = true)]
        [TestCase(false, "^Assets/Test", ExpectedResult = false)]
        [TestCase(true, "^Assets/Test2", ExpectedResult = true)]
        [TestCase(true, "^Assets/Test2/Test3", ExpectedResult = true)]
        public bool IsMatch_TargetIsFolder(bool matchWithFolder, string targetAssetPath)
        {
            var filter = new RegexBasedAssetFilter();
            filter.Condition = AssetFilterCondition.ContainsMatched;
            filter.MatchWithFolders = matchWithFolder;
            filter.AssetPathRegex.Value = "Assets/Test";
            filter.SetupForMatching();
            return filter.IsMatch(targetAssetPath, typeof(DefaultAsset), true, null, null);
        }

        [Test]
        public void Validate_ValidRegex_ReturnTrue()
        {
            var filter = new RegexBasedAssetFilter();
            filter.AssetPathRegex.Value = "^Assets/Test/.+";
            filter.SetupForMatching();

            Assert.That(filter.Validate(out _), Is.True);
        }

        [Test]
        public void Validate_InvalidRegex_ReturnFalse()
        {
            var filter = new RegexBasedAssetFilter();
            filter.AssetPathRegex.Value = "^Assets/(Test/.+";
            filter.SetupForMatching();

            Assert.That(filter.Validate(out _), Is.False);
        }
    }
}
