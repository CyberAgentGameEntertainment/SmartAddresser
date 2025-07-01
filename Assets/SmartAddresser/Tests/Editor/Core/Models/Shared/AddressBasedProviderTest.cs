using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared
{
    internal sealed class AddressBasedProviderTest
    {
        // AddressBasedProviderは抽象クラスなので、テスト用の具象クラスを作成
        private class TestAddressBasedProvider : AddressBasedProvider
        {
        }

        [Test]
        public void Provide_WithoutRegex_ReturnsOriginalAddress()
        {
            var provider = new TestAddressBasedProvider();
            provider.ReplaceWithRegex = false;
            provider.Setup();

            var result = provider.Provide("assets/textures/player");
            
            Assert.That(result, Is.EqualTo("assets/textures/player"));
        }

        [Test]
        public void Provide_WithRegex_ReturnsReplacedAddress()
        {
            var provider = new TestAddressBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"assets/(.+)/(.+)";
            provider.Replacement = "$2_$1";
            provider.Setup();

            var result = provider.Provide("assets/textures/player");
            
            Assert.That(result, Is.EqualTo("player_textures"));
        }

        [Test]
        public void Provide_WithRegexRemovePrefix_ReturnsModifiedAddress()
        {
            var provider = new TestAddressBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"^assets/";
            provider.Replacement = "";
            provider.Setup();

            var result = provider.Provide("assets/textures/player");
            
            Assert.That(result, Is.EqualTo("textures/player"));
        }

        [Test]
        public void Provide_WithEmptyAddress_ReturnsNull()
        {
            var provider = new TestAddressBasedProvider();
            provider.Setup();

            var result1 = provider.Provide("");
            var result2 = provider.Provide(null);
            
            Assert.That(result1, Is.Null);
            Assert.That(result2, Is.Null);
        }

        [Test]
        public void Provide_WithInvalidRegexPattern_ReturnsNull()
        {
            var provider = new TestAddressBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = "["; // 不正な正規表現パターン
            provider.Replacement = "replacement";
            provider.Setup();

            var result = provider.Provide("assets/textures/player");
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Provide_WithRegexThrowingException_ReturnsNull()
        {
            var provider = new TestAddressBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"(.+)";
            provider.Replacement = "$99"; // 存在しないグループ参照
            provider.Setup();

            var result = provider.Provide("assets/textures/player");
            
            // 正規表現の置換でエラーが発生した場合はnullを返す
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDescription_WithoutRegex_ReturnsBasicDescription()
        {
            var provider = new TestAddressBasedProvider();
            provider.ReplaceWithRegex = false;

            var description = provider.GetDescription();
            
            Assert.That(description, Is.EqualTo("Source: Address"));
        }

        [Test]
        public void GetDescription_WithRegex_ReturnsDetailedDescription()
        {
            var provider = new TestAddressBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"^assets/";
            provider.Replacement = "";

            var description = provider.GetDescription();
            
            Assert.That(description, Is.EqualTo("Source: Address, Regex: Replace \"^assets/\" with \"\""));
        }

        [Test]
        public void Setup_WithInvalidRegexPattern_HandlesGracefully()
        {
            var provider = new TestAddressBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = "["; // 不正な正規表現パターン
            provider.Replacement = "replacement";

            // Setupメソッドは例外をスローしない
            Assert.DoesNotThrow(() => provider.Setup());
        }

        [Test]
        public void Provide_MultipleCallsWithSamePattern_ReturnsSameResult()
        {
            var provider = new TestAddressBasedProvider();
            provider.ReplaceWithRegex = true;
            provider.Pattern = @"(.+)/(.+)";
            provider.Replacement = "$2/$1";
            provider.Setup();

            var result1 = provider.Provide("folder/file");
            var result2 = provider.Provide("folder/file");
            
            Assert.That(result1, Is.EqualTo("file/folder"));
            Assert.That(result2, Is.EqualTo("file/folder"));
        }
    }
}