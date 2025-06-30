using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    internal sealed class FindAssetsBasedAssetFilterTest
    {
        [Test]
        public void IsMatch_ValidFilter_ReturnTrue()
        {
            var filter = new FindAssetsBasedAssetFilter();
            filter.Filter = "t:texture tex_test";
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture64, typeof(Texture2D), false, null, null), Is.True);
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture128, typeof(Texture2D), false, null, null), Is.True);
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture256, typeof(Texture2D), false, null, null), Is.True);
        }

        [Test]
        public void IsMatch_ValidFilterWithValidFolder_ReturnTrue()
        {
            var filter = new FindAssetsBasedAssetFilter();
            filter.Filter = "t:texture tex_test";
            filter.TargetFolder.Value = AssetDatabase.LoadAssetAtPath<DefaultAsset>(TestAssetPaths.Shared.Folder);
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture64, typeof(Texture2D), false, null, null), Is.True);
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture128, typeof(Texture2D), false, null, null), Is.True);
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture256, typeof(Texture2D), false, null, null), Is.True);
        }

        [Test]
        public void IsMatch_InValidFilter_ReturnFalse()
        {
            var filter = new FindAssetsBasedAssetFilter();
            filter.Filter = "t:texture tex_test_notexists";
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture64, typeof(Texture2D), false, null, null), Is.False);
        }

        [Test]
        public void IsMatch_ValidFilterWithInvalidFolder_ReturnFalse()
        {
            var filter = new FindAssetsBasedAssetFilter();
            filter.Filter = "t:texture tex_test";
            filter.TargetFolder.Value = AssetDatabase.LoadAssetAtPath<DefaultAsset>(TestAssetPaths.Dummy.Folder);
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Dummy.PrefabDummy, typeof(GameObject), false, null, null), Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        public void IsMatch_FilterIsNullOrWhiteSpace_ReturnFalse(string filterText)
        {
            var filter = new FindAssetsBasedAssetFilter();
            filter.Filter = filterText;
            filter.SetupForMatching();
            Assert.That(filter.IsMatch(TestAssetPaths.Shared.Texture64, typeof(Texture2D), false, null, null), Is.False);
        }
    }
}
