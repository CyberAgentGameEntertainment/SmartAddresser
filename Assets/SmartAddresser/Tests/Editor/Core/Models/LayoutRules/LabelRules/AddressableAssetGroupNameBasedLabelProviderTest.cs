using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.LayoutRules.LabelRules
{
    internal sealed class AddressableAssetGroupNameBasedLabelProviderTest
    {
        private ILabelProvider _labelProvider;
        private AddressableAssetGroupNameBasedLabelProvider _provider;

        [SetUp]
        public void Setup()
        {
            _provider = new AddressableAssetGroupNameBasedLabelProvider();
            _labelProvider = _provider;
        }

        [Test]
        public void Provide_WithoutRegex_ReturnsGroupName()
        {
            _provider.ReplaceWithRegex = false;
            _labelProvider.Setup();

            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "TestGroup";

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "dummy/address", group);

            Assert.That(result, Is.EqualTo("TestGroup"));

            Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_WithRegex_ReturnsModifiedGroupName()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"Group_(.+)";
            _provider.Replacement = "$1_Label";
            _labelProvider.Setup();

            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "Group_Characters";

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "dummy/address", group);

            Assert.That(result, Is.EqualTo("Characters_Label"));

            Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_WithNullGroup_ReturnsNull()
        {
            _labelProvider.Setup();

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "dummy/address", null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithInvalidRegex_ReturnsNull()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = "[{";
            _provider.Replacement = "replacement";
            _labelProvider.Setup();

            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "TestGroup";

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "dummy/address", group);

            Assert.That(result, Is.Null);

            Object.DestroyImmediate(group);
        }

        [Test]
        public void GetDescription_WithoutRegex_ReturnsBasicDescription()
        {
            _provider.ReplaceWithRegex = false;

            var description = _labelProvider.GetDescription();

            Assert.That(description, Is.EqualTo("Source: Addressable Asset Group Name"));
        }

        [Test]
        public void GetDescription_WithRegex_ReturnsDetailedDescription()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"_v\d+$";
            _provider.Replacement = "";

            var description = _labelProvider.GetDescription();

            Assert.That(description,
                Is.EqualTo("Source: Addressable Asset Group Name, Regex: Replace \"_v\\d+$\" with \"\""));
        }
    }
}
