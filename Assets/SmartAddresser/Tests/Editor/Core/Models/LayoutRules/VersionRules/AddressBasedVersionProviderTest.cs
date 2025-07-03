using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;

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

            Assert.That(description,
                Is.EqualTo("Source: Address, Regex: Replace \"v(\\d+\\.\\d+\\.\\d+)\" with \"$1\""));
        }
    }
}
