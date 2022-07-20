using System;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.EntryRules.AddressRules;
using UnityEditor.VersionControl;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.EntryRules.AddressRules
{
    internal sealed class AssetPathBasedAddressProviderTest
    {
        [TestCase(AssetPathBasedAddressProvider.SourceType.FileName, ExpectedResult = "Dummy.asset")]
        [TestCase(AssetPathBasedAddressProvider.SourceType.FileNameWithoutExtensions, ExpectedResult = "Dummy")]
        [TestCase(AssetPathBasedAddressProvider.SourceType.FilePath, ExpectedResult = "Assets/Dummy.asset")]
        public string SourceType(AssetPathBasedAddressProvider.SourceType sourceType)
        {
            var providerImpl = new AssetPathBasedAddressProvider();
            var provider = (IAddressProvider)providerImpl;
            providerImpl.Source = sourceType;
            providerImpl.ReplaceWithRegex = false;
            provider.Setup();
            var address = provider.CreateAddress("Assets/Dummy.asset", typeof(ScriptableObject), false);
            return address;
        }

        [Test]
        public void ReplaceWithRegex()
        {
            var providerImpl = new AssetPathBasedAddressProvider();
            var provider = (IAddressProvider)providerImpl;
            providerImpl.Source = AssetPathBasedAddressProvider.SourceType.FilePath;
            providerImpl.ReplaceWithRegex = true;
            providerImpl.Pattern = "^Assets/(?<key>[a-zA-Z]{5}).asset$";
            providerImpl.Replacement = "${key}";
            provider.Setup();
            var address = provider.CreateAddress("Assets/Dummy.asset", typeof(ScriptableObject), false);
            Assert.That(address, Is.EqualTo("Dummy"));
            
        }
    }
}
