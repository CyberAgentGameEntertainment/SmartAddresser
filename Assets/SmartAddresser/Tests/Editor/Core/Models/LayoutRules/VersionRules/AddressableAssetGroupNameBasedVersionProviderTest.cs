using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.LayoutRules.VersionRules
{
    internal sealed class AddressableAssetGroupNameBasedVersionProviderTest
    {
        private AddressableAssetGroupNameBasedVersionProvider _provider;
        private IVersionProvider _versionProvider;

        [SetUp]
        public void Setup()
        {
            _provider = new AddressableAssetGroupNameBasedVersionProvider();
            _versionProvider = _provider;
        }

        [Test]
        public void Provide_WithoutRegex_ReturnsGroupName()
        {
            _provider.ReplaceWithRegex = false;
            _versionProvider.Setup();

            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "Version_1.0.0";

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "dummy/address", group);

            Assert.That(result, Is.EqualTo("Version_1.0.0"));

            Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_WithRegex_ReturnsModifiedGroupName()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"Version_(.+)";
            _provider.Replacement = "$1";
            _versionProvider.Setup();

            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "Version_2.5.1";

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "dummy/address", group);

            Assert.That(result, Is.EqualTo("2.5.1"));

            Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_WithNullGroup_ReturnsNull()
        {
            _versionProvider.Setup();

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "dummy/address", null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithInvalidRegex_ReturnsNull()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = "({[";
            _provider.Replacement = "replacement";
            _versionProvider.Setup();

            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "Version_1.0.0";

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "dummy/address", group);

            Assert.That(result, Is.Null);

            Object.DestroyImmediate(group);
        }

        [Test]
        public void GetDescription_WithoutRegex_ReturnsBasicDescription()
        {
            _provider.ReplaceWithRegex = false;

            var description = _versionProvider.GetDescription();

            Assert.That(description, Is.EqualTo("Source: Addressable Asset Group Name"));
        }

        [Test]
        public void GetDescription_WithRegex_ReturnsDetailedDescription()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"v(.+)_Group";
            _provider.Replacement = "$1";

            var description = _versionProvider.GetDescription();

            Assert.That(description,
                Is.EqualTo("Source: Addressable Asset Group Name, Regex: Replace \"v(.+)_Group\" with \"$1\""));
        }
    }
}
