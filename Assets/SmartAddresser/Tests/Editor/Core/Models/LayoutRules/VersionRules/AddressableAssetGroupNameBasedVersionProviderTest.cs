using System;
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

            UnityEngine.Object.DestroyImmediate(group);
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

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_WithNullGroup_ReturnsNull()
        {
            _versionProvider.Setup();

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "dummy/address", null);
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithEmptyGroupName_ReturnsNull()
        {
            _versionProvider.Setup();
            
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "";

            var result = _versionProvider.Provide("dummy/path", typeof(object), false, "dummy/address", group);
            
            Assert.That(result, Is.Null);

            UnityEngine.Object.DestroyImmediate(group);
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

            UnityEngine.Object.DestroyImmediate(group);
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
            
            Assert.That(description, Is.EqualTo("Source: Addressable Asset Group Name, Regex: Replace \"v(.+)_Group\" with \"$1\""));
        }

        [Test]
        public void Provide_IgnoresAssetPathAndType()
        {
            // AddressableAssetGroupNameBasedVersionProviderはアセットパスやタイプを使用しない
            _versionProvider.Setup();
            
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.Name = "v1.0.0";

            var result1 = _versionProvider.Provide("path1", typeof(Texture2D), false, "addr1", group);
            var result2 = _versionProvider.Provide("path2", typeof(GameObject), true, "addr2", group);
            
            Assert.That(result1, Is.EqualTo("v1.0.0"));
            Assert.That(result2, Is.EqualTo("v1.0.0"));

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void Provide_WithComplexVersionPattern_WorksCorrectly()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @"^(.+)_v(\d+)_(\d+)_(\d+)(_[a-z]+)?$";
            _provider.Replacement = "$2.$3.$4$5";
            _versionProvider.Setup();
            
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group3 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group4 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group1.Name = "Characters_v1_0_0";
            group2.Name = "Weapons_v2_1_0_beta";
            group3.Name = "Items_v3_2_5";
            group4.Name = "SimpleGroup";

            var result1 = _versionProvider.Provide("dummy", typeof(object), false, "dummy", group1);
            var result2 = _versionProvider.Provide("dummy", typeof(object), false, "dummy", group2);
            var result3 = _versionProvider.Provide("dummy", typeof(object), false, "dummy", group3);
            var result4 = _versionProvider.Provide("dummy", typeof(object), false, "dummy", group4);
            
            Assert.That(result1, Is.EqualTo("1.0.0"));
            Assert.That(result2, Is.EqualTo("2.1.0_beta"));
            Assert.That(result3, Is.EqualTo("3.2.5"));
            Assert.That(result4, Is.EqualTo("SimpleGroup")); // パターンにマッチしない

            UnityEngine.Object.DestroyImmediate(group1);
            UnityEngine.Object.DestroyImmediate(group2);
            UnityEngine.Object.DestroyImmediate(group3);
            UnityEngine.Object.DestroyImmediate(group4);
        }

        [Test]
        public void Provide_WithSemanticVersioning_ExtractsCorrectly()
        {
            _provider.ReplaceWithRegex = true;
            _provider.Pattern = @".*\[(\d+\.\d+\.\d+(?:-[a-zA-Z]+(?:\.\d+)?)?)\].*";
            _provider.Replacement = "$1";
            _versionProvider.Setup();
            
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group3 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group1.Name = "Assets[1.0.0]";
            group2.Name = "BetaAssets[2.0.0-beta.1]";
            group3.Name = "Production[3.1.0-rc]";

            var result1 = _versionProvider.Provide("dummy", typeof(object), false, "dummy", group1);
            var result2 = _versionProvider.Provide("dummy", typeof(object), false, "dummy", group2);
            var result3 = _versionProvider.Provide("dummy", typeof(object), false, "dummy", group3);
            
            Assert.That(result1, Is.EqualTo("1.0.0"));
            Assert.That(result2, Is.EqualTo("2.0.0-beta.1"));
            Assert.That(result3, Is.EqualTo("3.1.0-rc"));

            UnityEngine.Object.DestroyImmediate(group1);
            UnityEngine.Object.DestroyImmediate(group2);
            UnityEngine.Object.DestroyImmediate(group3);
        }
    }
}