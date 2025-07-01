using System;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.LayoutRules.VersionRules
{
    internal sealed class AddressBasedVersionProviderTest
    {
        private AddressBasedVersionProvider _provider;
        private IVersionProvider _versionProvider;

        [SetUp]
        public void Setup()
        {
            _provider = new AddressBasedVersionProvider();
            _versionProvider = _provider;
        }

        [Test]
        public void Provide_WithoutRegex_ReturnsAddress()
        {
            _provider.ReplaceWithRegex = false;
            _versionProvider.Setup();

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "v1.0.0/character", null);
            
            Assert.That(result, Is.EqualTo("v1.0.0/character"));
        }

        [Test]
        public void Provide_WithRegex_ReturnsModifiedAddress()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"v(\d+\.\d+\.\d+)/.*";
            _provider.Replacement = "$1";
            _versionProvider.Setup();

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "v1.2.3/character/player", null);
            
            Assert.That(result, Is.EqualTo("1.2.3"));
        }

        [Test]
        public void Provide_WithNullAddress_ReturnsNull()
        {
            _versionProvider.Setup();

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, null, null);
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithEmptyAddress_ReturnsNull()
        {
            _versionProvider.Setup();

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "", null);
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithInvalidRegex_ReturnsNull()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = "[(";
            _provider.Replacement = "replacement";
            _versionProvider.Setup();

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "v1.0.0/character", null);
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDescription_WithoutRegex_ReturnsBasicDescription()
        {
            _provider.ReplaceWithRegex = false;

            var description = _versionProvider.GetDescription();
            
            Assert.That(description, Is.EqualTo("Source: Address"));
        }

        [Test]
        public void GetDescription_WithRegex_ReturnsDetailedDescription()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"v(\d+\.\d+\.\d+)";
            _provider.Replacement = "$1";

            var description = _versionProvider.GetDescription();
            
            Assert.That(description, Is.EqualTo("Source: Address, Regex: Replace \"v(\\d+\\.\\d+\\.\\d+)\" with \"$1\""));
        }

        [Test]
        public void Provide_WithAddressableAssetGroup_IgnoresGroup()
        {
            // AddressBasedVersionProviderはAddressableAssetGroupを使用しない
            _versionProvider.Setup();
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "v2.0.0", group);
            
            Assert.That(result, Is.EqualTo("v2.0.0"));

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_ExtractVersionFromPath_WorksCorrectly()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @".*/v(\d+)_(\d+)_(\d+)/.*";
            _provider.Replacement = "$1.$2.$3";
            _versionProvider.Setup();

            var result1 = _versionProvider.Provide("dummy", typeof(object), false, "assets/v1_0_0/textures/player", null);
            var result2 = _versionProvider.Provide("dummy", typeof(object), false, "models/v2_5_1/characters", null);
            
            Assert.That(result1, Is.EqualTo("1.0.0"));
            Assert.That(result2, Is.EqualTo("2.5.1"));
        }

        [Test]
        public void Provide_WithSemanticVersionPattern_WorksCorrectly()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @".*@(\d+\.\d+\.\d+(-[a-z]+\.\d+)?)$";
            _provider.Replacement = "$1";
            _versionProvider.Setup();

            var result1 = _versionProvider.Provide("dummy", typeof(object), false, "character/player@1.0.0", null);
            var result2 = _versionProvider.Provide("dummy", typeof(object), false, "weapon/sword@2.1.0-beta.1", null);
            var result3 = _versionProvider.Provide("dummy", typeof(object), false, "item/potion", null);
            
            Assert.That(result1, Is.EqualTo("1.0.0"));
            Assert.That(result2, Is.EqualTo("2.1.0-beta.1"));
            Assert.That(result3, Is.EqualTo("item/potion")); // パターンにマッチしない
        }
    }
}