using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.LayoutRules.Shared
{
    internal sealed class AssetPathBasedProviderTest
    {
        [TestCase(PartialAssetPathType.FileName, ExpectedResult = "Dummy.asset")]
        [TestCase(PartialAssetPathType.FileNameWithoutExtensions, ExpectedResult = "Dummy")]
        [TestCase(PartialAssetPathType.AssetPath, ExpectedResult = "Assets/Dummy.asset")]
        public string SourceType(PartialAssetPathType sourceType)
        {
            var provider = new FakeAssetPathBasedProvider();
            provider.Source = sourceType;
            provider.ReplaceWithRegex = false;
            provider.Setup();
            var address = provider.Provide("Assets/Dummy.asset", typeof(ScriptableObject), false);
            return address;
        }

        [Test]
        public void ReplaceWithRegex()
        {
            var provider = new FakeAssetPathBasedProvider();
            provider.Source = PartialAssetPathType.AssetPath;
            provider.ReplaceWithRegex = true;
            provider.Pattern = "^Assets/(?<key>[a-zA-Z]{5}).asset$";
            provider.Replacement = "${key}";
            provider.Setup();
            var address = provider.Provide("Assets/Dummy.asset", typeof(ScriptableObject), false);
            Assert.That(address, Is.EqualTo("Dummy"));
        }

        private sealed class FakeAssetPathBasedProvider : AssetPathBasedProvider
        {
        }
    }
}
