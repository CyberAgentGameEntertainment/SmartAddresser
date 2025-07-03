using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.LayoutRules.LabelRules
{
    internal sealed class AddressBasedLabelProviderTest
    {
        private ILabelProvider _labelProvider;
        private AddressBasedLabelProvider _provider;

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
    }
}
