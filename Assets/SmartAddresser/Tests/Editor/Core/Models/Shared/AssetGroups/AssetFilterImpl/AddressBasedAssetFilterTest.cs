using System;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    internal sealed class AddressBasedAssetFilterTest
    {
        private AddressBasedAssetFilter _filter;

        [SetUp]
        public void Setup()
        {
            _filter = new AddressBasedAssetFilter();
        }

        [Test]
        public void IsMatch_WithSingleRegex_ContainsMatched_ReturnsTrue()
        {
            _filter.Condition = AssetFilterCondition.ContainsMatched;
            _filter.AddressRegex.Value = "character/.*";
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy/path", typeof(object), false, "character/player", null);
            
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithSingleRegex_ContainsMatched_ReturnsFalse()
        {
            _filter.Condition = AssetFilterCondition.ContainsMatched;
            _filter.AddressRegex.Value = "character/.*";
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy/path", typeof(object), false, "weapon/sword", null);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMatch_WithMultipleRegex_ContainsMatched_ReturnsTrue()
        {
            _filter.Condition = AssetFilterCondition.ContainsMatched;
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue("character/.*");
            _filter.AddressRegex.AddValue("weapon/.*");
            _filter.SetupForMatching();

            var result1 = _filter.IsMatch("dummy", typeof(object), false, "character/player", null);
            var result2 = _filter.IsMatch("dummy", typeof(object), false, "weapon/sword", null);
            var result3 = _filter.IsMatch("dummy", typeof(object), false, "item/potion", null);
            
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
            Assert.That(result3, Is.False);
        }

        [Test]
        public void IsMatch_WithMultipleRegex_MatchAll_ReturnsTrue()
        {
            _filter.Condition = AssetFilterCondition.MatchAll;
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue(".*player.*");
            _filter.AddressRegex.AddValue("character/.*");
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy", typeof(object), false, "character/player", null);
            
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithMultipleRegex_MatchAll_ReturnsFalse()
        {
            _filter.Condition = AssetFilterCondition.MatchAll;
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue(".*player.*");
            _filter.AddressRegex.AddValue("character/.*");
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy", typeof(object), false, "character/enemy", null);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMatch_WithMultipleRegex_ContainsUnmatched_ReturnsTrue()
        {
            _filter.Condition = AssetFilterCondition.ContainsUnmatched;
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue("character/.*");
            _filter.AddressRegex.AddValue("weapon/.*");
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy", typeof(object), false, "character/player", null);
            
            // character/playerは"weapon/.*"にマッチしないので、true
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithMultipleRegex_NotMatchAll_ReturnsTrue()
        {
            _filter.Condition = AssetFilterCondition.NotMatchAll;
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue("character/.*");
            _filter.AddressRegex.AddValue("weapon/.*");
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy", typeof(object), false, "item/potion", null);
            
            Assert.That(result, Is.True);
        }
        
        [Test]
        public void IsMatch_WithFolder_WhenMatchWithFoldersIsFalse_ReturnsFalse()
        {
            _filter.MatchWithFolders = false;
            _filter.Condition = AssetFilterCondition.ContainsMatched;
            _filter.AddressRegex.Value = ".*";
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy", typeof(object), true, "folder/path", null);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMatch_WithFolder_WhenMatchWithFoldersIsTrue_ReturnsTrue()
        {
            _filter.MatchWithFolders = true;
            _filter.Condition = AssetFilterCondition.ContainsMatched;
            _filter.AddressRegex.Value = ".*";
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy", typeof(object), true, "folder/path", null);
            
            Assert.That(result, Is.True);
        }

        [Test]
        public void SetupForMatching_WithInvalidRegex_DoesNotThrow()
        {
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue("valid.*");
            _filter.AddressRegex.AddValue("[");  // 不正な正規表現
            _filter.AddressRegex.AddValue("another.*");

            Assert.DoesNotThrow(() => _filter.SetupForMatching());
        }

        [Test]
        public void Validate_WithInvalidRegex_ReturnsFalse()
        {
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue("valid.*");
            _filter.AddressRegex.AddValue("[");  // 不正な正規表現
            _filter.AddressRegex.AddValue("({");  // 別の不正な正規表現
            _filter.SetupForMatching();

            var result = _filter.Validate(out var error);
            
            Assert.That(result, Is.False);
            Assert.That(error, Is.Not.Null);
            Assert.That(error.ErrorMessages.Count, Is.EqualTo(2));
            Assert.That(error.ErrorMessages[0], Does.Contain("Invalid regex string: ["));
            Assert.That(error.ErrorMessages[1], Does.Contain("Invalid regex string: ({"));
        }

        [Test]
        public void Validate_WithValidRegex_ReturnsTrue()
        {
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue("character/.*");
            _filter.AddressRegex.AddValue("weapon/.*");
            _filter.SetupForMatching();

            var result = _filter.Validate(out var error);
            
            Assert.That(result, Is.True);
            Assert.That(error, Is.Null);
        }

        [Test]
        public void GetDescription_WithSingleRegex_ContainsMatched()
        {
            _filter.Condition = AssetFilterCondition.ContainsMatched;
            _filter.AddressRegex.Value = "character/.*";

            var description = _filter.GetDescription();
            
            Assert.That(description, Is.EqualTo("Address Match: character/.*"));
        }

        [Test]
        public void GetDescription_WithMultipleRegex_MatchAll()
        {
            _filter.Condition = AssetFilterCondition.MatchAll;
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue(".*player.*");
            _filter.AddressRegex.AddValue("character/.*");

            var description = _filter.GetDescription();
            
            Assert.That(description, Is.EqualTo("Address Match: ( .*player.* && character/.* )"));
        }

        [Test]
        public void GetDescription_WithMultipleRegex_NotMatchAll()
        {
            _filter.Condition = AssetFilterCondition.NotMatchAll;
            _filter.AddressRegex.IsListMode = true;
            _filter.AddressRegex.AddValue("enemy/.*");
            _filter.AddressRegex.AddValue("boss/.*");

            var description = _filter.GetDescription();
            
            Assert.That(description, Is.EqualTo("Address Not Match: ( enemy/.* && boss/.* )"));
        }

        [Test]
        public void GetDescription_WithEmptyRegexList_ReturnsEmptyString()
        {
            var description = _filter.GetDescription();
            
            Assert.That(description, Is.Empty);
        }

        [Test]
        public void IsMatch_WithAddressableAssetGroup_IgnoresGroup()
        {
            // AddressBasedAssetFilterはAddressableAssetGroupを使用しない
            _filter.Condition = AssetFilterCondition.ContainsMatched;
            _filter.AddressRegex.Value = "test/.*";
            _filter.SetupForMatching();
            
            var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();

            var result = _filter.IsMatch("dummy", typeof(object), false, "test/address", group);
            
            Assert.That(result, Is.True);

            UnityEngine.Object.DestroyImmediate(group);
        }

        [Test]
        public void IsMatch_EmptyRegexList_AlwaysFalseForContainsMatched()
        {
            _filter.Condition = AssetFilterCondition.ContainsMatched;
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy", typeof(object), false, "any/address", null);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMatch_EmptyRegexList_AlwaysTrueForMatchAll()
        {
            _filter.Condition = AssetFilterCondition.MatchAll;
            _filter.SetupForMatching();

            var result = _filter.IsMatch("dummy", typeof(object), false, "any/address", null);
            
            // 空のリストに対してすべてマッチは論理的にtrue
            Assert.That(result, Is.True);
        }
    }
}
