using System;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.LayoutRules.LabelRules
{
    internal sealed class AddressBasedLabelProviderTest
    {
        private AddressBasedLabelProvider _provider;
        private ILabelProvider _labelProvider;

        [SetUp]
        public void Setup()
        {
            _provider = new AddressBasedLabelProvider();
            _labelProvider = _provider;
        }

        [Test]
        public void Provide_WithoutRegex_ReturnsAddress()
        {
            _provider.ReplaceWithRegex = false;
            _labelProvider.Setup();

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "character/player", null);
            
            Assert.That(result, Is.EqualTo("character/player"));
        }

        [Test]
        public void Provide_WithRegex_ReturnsModifiedAddress()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"character/(.+)";
            _provider.Replacement = "$1_label";
            _labelProvider.Setup();

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "character/player", null);
            
            Assert.That(result, Is.EqualTo("player_label"));
        }

        [Test]
        public void Provide_WithNullAddress_ReturnsNull()
        {
            _labelProvider.Setup();

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, null, null);
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithEmptyAddress_ReturnsNull()
        {
            _labelProvider.Setup();

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "", null);
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithInvalidRegex_ReturnsNull()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = "[";
            _provider.Replacement = "replacement";
            _labelProvider.Setup();

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "character/player", null);
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDescription_WithoutRegex_ReturnsBasicDescription()
        {
            _provider.ReplaceWithRegex = false;

            var description = _labelProvider.GetDescription();
            
            Assert.That(description, Is.EqualTo("Source: Address"));
        }

        [Test]
        public void GetDescription_WithRegex_ReturnsDetailedDescription()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"^prefix/";
            _provider.Replacement = "";

            var description = _labelProvider.GetDescription();
            
            Assert.That(description, Is.EqualTo("Source: Address, Regex: Replace \"^prefix/\" with \"\""));
        }

        [Test]
        public void Provide_WithAddressableAssetGroup_IgnoresGroup()
        {
            // AddressBasedLabelProviderはAddressableAssetGroupを使用しない
            _labelProvider.Setup();
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "test/address", group);
            
            Assert.That(result, Is.EqualTo("test/address"));

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_WithComplexRegexPattern_WorksCorrectly()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"assets/([^/]+)/([^/]+)";
            _provider.Replacement = "$2_$1";
            _labelProvider.Setup();

            var result1 = _labelProvider.Provide("dummy", typeof(object), false, "assets/textures/player", null);
            var result2 = _labelProvider.Provide("dummy", typeof(object), false, "assets/models/enemy", null);
            
            Assert.That(result1, Is.EqualTo("player_textures"));
            Assert.That(result2, Is.EqualTo("enemy_models"));
        }
    }
}