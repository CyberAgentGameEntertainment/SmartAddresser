using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.LayoutRules.Shared
{
    internal sealed class AssetPathBasedProviderTest
    {
        [TestCase(PartialAssetPathType.AssetName, ExpectedResult = "Dummy.asset")]
        [TestCase(PartialAssetPathType.AssetNameWithoutExtensions, ExpectedResult = "Dummy")]
        [TestCase(PartialAssetPathType.AssetPath, ExpectedResult = "Assets/Dummy.asset")]
        public string SourceType(PartialAssetPathType sourceType)
        {
            var providerImpl = new FakeAssetPathBasedProvider();
            var provider = (IProvider<string>)providerImpl;
            providerImpl.Source = sourceType;
            providerImpl.ReplaceWithRegex = false;
            provider.Setup();
            var address = provider.Provide("Assets/Dummy.asset", typeof(ScriptableObject), false);
            return address;
        }

        [Test]
        public void ReplaceWithRegex()
        {
            var providerImpl = new FakeAssetPathBasedProvider();
            var provider = (IProvider<string>)providerImpl;
            providerImpl.Source = PartialAssetPathType.AssetPath;
            providerImpl.ReplaceWithRegex = true;
            providerImpl.Pattern = "^Assets/(?<key>[a-zA-Z]{5}).asset$";
            providerImpl.Replacement = "${key}";
            provider.Setup();
            var address = provider.Provide("Assets/Dummy.asset", typeof(ScriptableObject), false);
            Assert.That(address, Is.EqualTo("Dummy"));
        }

        private sealed class FakeAssetPathBasedProvider : AssetPathBasedProvider
        {
        }
    }
}
