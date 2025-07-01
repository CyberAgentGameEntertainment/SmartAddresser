using System;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.LayoutRules.LabelRules
{
    internal sealed class AddressableAssetGroupNameBasedLabelProviderTest
    {
        private AddressableAssetGroupNameBasedLabelProvider _provider;
        private ILabelProvider _labelProvider;

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

            UnityEngine.Object.DestroyImmediate(group);
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

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_WithNullGroup_ReturnsNull()
        {
            _labelProvider.Setup();

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "dummy/address", null);
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithEmptyGroupName_ReturnsNull()
        {
            _labelProvider.Setup();
            
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "";

            var result = _labelProvider.Provide("dummy/path", typeof(object), false, "dummy/address", group);
            
            Assert.That(result, Is.Null);

            UnityEngine.Object.DestroyImmediate(group);
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

            UnityEngine.Object.DestroyImmediate(group);
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
            
            Assert.That(description, Is.EqualTo("Source: Addressable Asset Group Name, Regex: Replace \"_v\\d+$\" with \"\""));
        }

        [Test]
        public void Provide_IgnoresAssetPathAndType()
        {
            // AddressableAssetGroupNameBasedLabelProviderはアセットパスやタイプを使用しない
            _labelProvider.Setup();
            
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "TestGroup";

            var result1 = _labelProvider.Provide("path1", typeof(Texture2D), false, "addr1", group);
            var result2 = _labelProvider.Provide("path2", typeof(GameObject), true, "addr2", group);
            
            Assert.That(result1, Is.EqualTo("TestGroup"));
            Assert.That(result2, Is.EqualTo("TestGroup"));

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_WithComplexRegexPattern_WorksCorrectly()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"^(Dev|Test|Prod)_(.+)_Group$";
            _provider.Replacement = "$2_$1";
            _labelProvider.Setup();
            
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group3 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group1.Name = "Dev_Characters_Group";
            group2.Name = "Test_Weapons_Group";
            group3.Name = "MainAssets";

            var result1 = _labelProvider.Provide("dummy", typeof(object), false, "dummy", group1);
            var result2 = _labelProvider.Provide("dummy", typeof(object), false, "dummy", group2);
            var result3 = _labelProvider.Provide("dummy", typeof(object), false, "dummy", group3);
            
            Assert.That(result1, Is.EqualTo("Characters_Dev"));
            Assert.That(result2, Is.EqualTo("Weapons_Test"));
            Assert.That(result3, Is.EqualTo("MainAssets")); // パターンにマッチしない

            UnityEngine.Object.DestroyImmediate(group1);
            UnityEngine.Object.DestroyImmediate(group2);
            UnityEngine.Object.DestroyImmediate(group3);
        }
    }
}