using System;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    internal sealed class AddressableAssetGroupBasedAssetFilterTest
    {
        private AddressableAssetGroupBasedAssetFilter _filter;

        [SetUp]
        public void Setup()
        {
            _filter = new AddressableAssetGroupBasedAssetFilter();
        }

        [Test]
        public void IsMatch_WithMatchingGroup_ReturnsTrue()
        {
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group1.Name = "Group1";
            group2.Name = "Group2";
            
            _filter.Groups.IsListMode = true;
            _filter.Groups.AddValue(group1);
            _filter.Groups.AddValue(group2);
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy/path", typeof(object), false, "dummy/address", group1);
            
            Assert.That(result, Is.True);

            UnityEngine.Object.DestroyImmediate(group1);
            UnityEngine.Object.DestroyImmediate(group2);
        }

        [Test]
        public void IsMatch_WithNonMatchingGroup_ReturnsFalse()
        {
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group3 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group1.Name = "Group1";
            group2.Name = "Group2";
            group3.Name = "Group3";
            
            _filter.Groups.IsListMode = true;
            _filter.Groups.AddValue(group1);
            _filter.Groups.AddValue(group2);
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy/path", typeof(object), false, "dummy/address", group3);
            
            Assert.That(result, Is.False);

            UnityEngine.Object.DestroyImmediate(group1);
            UnityEngine.Object.DestroyImmediate(group2);
            UnityEngine.Object.DestroyImmediate(group3);
        }

        [Test]
        public void IsMatch_WithNullGroup_ReturnsFalse()
        {
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            _filter.Groups.Value = group1;
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy/path", typeof(object), false, "dummy/address", null);
            
            Assert.That(result, Is.False);

            UnityEngine.Object.DestroyImmediate(group1);
        }

        [Test]
        public void IsMatch_WithEmptyGroupList_ReturnsFalse()
        {
            _filter.SetupForMatching();
            
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();

            var result = _filter.IsMatch("dummy/path", typeof(object), false, "dummy/address", group);
            
            Assert.That(result, Is.False);

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void Validate_WithValidGroups_ReturnsTrue()
        {
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            
            _filter.Groups.IsListMode = true;
            _filter.Groups.AddValue(group1);
            _filter.Groups.AddValue(group2);
            _filter.SetupForMatching();

            var result = _filter.Validate(out var error);
            
            Assert.That(result, Is.True);
            Assert.That(error, Is.Null);

            UnityEngine.Object.DestroyImmediate(group1);
            UnityEngine.Object.DestroyImmediate(group2);
        }

        [Test]
        public void Validate_WithNullGroup_ReturnsFalse()
        {
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            
            _filter.Groups.IsListMode = true;
            _filter.Groups.AddValue(group1);
            _filter.Groups.AddValue(null);
            _filter.SetupForMatching();

            var result = _filter.Validate(out var error);
            
            Assert.That(result, Is.False);
            Assert.That(error, Is.Not.Null);
            Assert.That(error.ErrorMessages[0], Does.Contain("null reference groups"));

            UnityEngine.Object.DestroyImmediate(group1);
        }

        [Test]
        public void GetDescription_WithSingleGroup()
        {
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.name = "TestGroup";
            
            _filter.Groups.Value = group;

            var description = _filter.GetDescription();
            
            Assert.That(description, Is.EqualTo("Addressable Group: TestGroup"));

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void GetDescription_WithMultipleGroups()
        {
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group1.name = "Group1";
            group2.name = "Group2";
            
            _filter.Groups.IsListMode = true;
            _filter.Groups.AddValue(group1);
            _filter.Groups.AddValue(group2);

            var description = _filter.GetDescription();
            
            Assert.That(description, Is.EqualTo("Addressable Group: ( Group1 || Group2 )"));

            UnityEngine.Object.DestroyImmediate(group1);
            UnityEngine.Object.DestroyImmediate(group2);
        }

        [Test]
        public void GetDescription_WithNullGroup_IgnoresNull()
        {
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            group.name = "ValidGroup";
            
            _filter.Groups.IsListMode = true;
            _filter.Groups.AddValue(group);
            _filter.Groups.AddValue(null);

            var description = _filter.GetDescription();
            
            Assert.That(description, Is.EqualTo("Addressable Group: ValidGroup"));

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void GetDescription_WithEmptyGroupList_ReturnsEmptyString()
        {
            var description = _filter.GetDescription();
            
            Assert.That(description, Is.Empty);
        }

        [Test]
        public void IsMatch_IgnoresAssetPathAndType()
        {
            // AddressableAssetGroupBasedAssetFilterはアセットパスやタイプを使用しない
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            _filter.Groups.Value = group;
            _filter.SetupForMatching();

            var result1 = _filter.IsMatch("path1", typeof(Texture2D), false, "addr1", group);
            var result2 = _filter.IsMatch("path2", typeof(GameObject), true, "addr2", group);
            var result3 = _filter.IsMatch(null, null, false, null, group);
            
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
            Assert.That(result3, Is.True);

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void SetupForMatching_HandlesDuplicateGroups()
        {
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            
            // 同じグループを複数回追加
            _filter.Groups.IsListMode = true;
            _filter.Groups.AddValue(group);
            _filter.Groups.AddValue(group);
            _filter.Groups.AddValue(group);
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy", typeof(object), false, "dummy", group);
            
            // HashSetにより重複は自動的に処理される
            Assert.That(result, Is.True);

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void IsMatch_WithMultipleGroupsIncludingNull_WorksCorrectly()
        {
            var group1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            
            _filter.Groups.IsListMode = true;
            _filter.Groups.AddValue(group1);
            _filter.Groups.AddValue(null);
            _filter.Groups.AddValue(group2);
            _filter.SetupForMatching();

            var result1 = _filter.IsMatch("dummy", typeof(object), false, "dummy", group1);
            var result2 = _filter.IsMatch("dummy", typeof(object), false, "dummy", group2);
            
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);

            UnityEngine.Object.DestroyImmediate(group1);
            UnityEngine.Object.DestroyImmediate(group2);
        }
    }
}