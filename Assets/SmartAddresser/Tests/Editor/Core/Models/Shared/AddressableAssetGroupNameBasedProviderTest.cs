using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared
{
    internal sealed class AddressableAssetGroupNameBasedProviderTest
    {
        // AddressableAssetGroupNameBasedProviderは抽象クラスなので、テスト用の具象クラスを作成
        private class TestAddressableAssetGroupNameBasedProvider : AddressableAssetGroupNameBasedProvider
        {
        }

        [Test]
        public void Provide_WithoutRegex_ReturnsOriginalGroupName()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = false;
            provider.Setup();

            var result = provider.Provide("DefaultGroup");
            
            Assert.That(result, Is.EqualTo("DefaultGroup"));
        }

        [Test]
        public void Provide_WithRegex_ReturnsReplacedGroupName()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"Group_(.+)";
            provider.Replacement = "$1_Modified";
            provider.Setup();

            var result = provider.Provide("Group_Assets");
            
            Assert.That(result, Is.EqualTo("Assets_Modified"));
        }

        [Test]
        public void Provide_WithRegexRemovePrefix_ReturnsModifiedGroupName()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"^Prefix_";
            provider.Replacement = "";
            provider.Setup();

            var result = provider.Provide("Prefix_MainGroup");
            
            Assert.That(result, Is.EqualTo("MainGroup"));
        }

        [Test]
        public void Provide_WithEmptyGroupName_ReturnsNull()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.Setup();

            var result1 = provider.Provide("");
            var result2 = provider.Provide(null);
            
            Assert.That(result1, Is.Null);
            Assert.That(result2, Is.Null);
        }

        [Test]
        public void Provide_WithInvalidRegexPattern_ReturnsNull()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = "[("; // 不正な正規表現パターン
            provider.Replacement = "replacement";
            provider.Setup();

            var result = provider.Provide("TestGroup");
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithRegexThrowingException_ReturnsNull()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"(.+)";
            provider.Replacement = "$999"; // 存在しないグループ参照
            provider.Setup();

            var result = provider.Provide("TestGroup");
            
            // 正規表現の置換でエラーが発生した場合はnullを返す
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDescription_WithoutRegex_ReturnsBasicDescription()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = false;

            var description = provider.GetDescription();
            
            Assert.That(description, Is.EqualTo("Source: Addressable Asset Group Name"));
        }

        [Test]
        public void GetDescription_WithRegex_ReturnsDetailedDescription()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"^Group_";
            provider.Replacement = "";

            var description = provider.GetDescription();
            
            Assert.That(description, Is.EqualTo("Source: Addressable Asset Group Name, Regex: Replace \"^Group_\" with \"\""));
        }

        [Test]
        public void Setup_WithInvalidRegexPattern_HandlesGracefully()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = "[("; // 不正な正規表現パターン
            provider.Replacement = "replacement";

            // Setupメソッドは例外をスローしない
            Assert.DoesNotThrow(() => provider.Setup());
        }

        [Test]
        public void Provide_WithComplexRegexPattern_WorksCorrectly()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"^(Dev|Test)_(.+)_Group$";
            provider.Replacement = "$2";
            provider.Setup();

            var result1 = provider.Provide("Dev_Characters_Group");
            var result2 = provider.Provide("Test_Weapons_Group");
            var result3 = provider.Provide("Production_Assets_Group");
            
            Assert.That(result1, Is.EqualTo("Characters"));
            Assert.That(result2, Is.EqualTo("Weapons"));
            Assert.That(result3, Is.EqualTo("Production_Assets_Group")); // パターンにマッチしない
        }

        [Test]
        public void Provide_MultipleCallsWithSamePattern_ReturnsSameResult()
        {
            var provider = new TestAddressableAssetGroupNameBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"_v\d+$";
            provider.Replacement = "";
            provider.Setup();

            var result1 = provider.Provide("Assets_v1");
            var result2 = provider.Provide("Assets_v1");
            var result3 = provider.Provide("Assets_v2");
            
            Assert.That(result1, Is.EqualTo("Assets"));
            Assert.That(result2, Is.EqualTo("Assets"));
            Assert.That(result3, Is.EqualTo("Assets"));
        }
    }
}