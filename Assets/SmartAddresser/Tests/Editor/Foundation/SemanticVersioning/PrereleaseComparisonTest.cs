using NUnit.Framework;
using SmartAddresser.Editor.Foundation.SemanticVersioning;

namespace SmartAddresser.Tests.Editor.Foundation.SemanticVersioning
{
    internal sealed class PrereleaseComparisonTest
    {
        [TestCase("1.0.0-alpha", "1.0.0-alpha", ExpectedResult = true)]
        [TestCase("1.0.0-alpha", "1.0.0-alpha.1", ExpectedResult = false)]
        [TestCase("1.0.0-alpha+b1", "1.0.0-alpha+b2", ExpectedResult = true)]
        public bool Equal(string a, string b)
        {
            var versionA = Version.Create(a);
            var versionB = Version.Create(b);
            return versionA == versionB;
        }

        [TestCase("1.0.0-alpha", "1.0.0-alpha.1", ExpectedResult = true)]
        [TestCase("1.0.0-alpha", "1.0.0-alpha", ExpectedResult = false)]
        [TestCase("1.0.0-alpha+b1", "1.0.0-alpha+b2", ExpectedResult = false)]
        public bool NotEqual(string a, string b)
        {
            var versionA = Version.Create(a);
            var versionB = Version.Create(b);
            return versionA != versionB;
        }

        [TestCase("1.0.0-alpha", "1.0.0-alpha.1", ExpectedResult = true)]
        [TestCase("1.0.0-alpha.1", "1.0.0-alpha.beta", ExpectedResult = true)]
        [TestCase("1.0.0-alpha.beta", "1.0.0-beta", ExpectedResult = true)]
        [TestCase("1.0.0-beta", "1.0.0-beta.2", ExpectedResult = true)]
        [TestCase("1.0.0-beta.2", "1.0.0-beta.11", ExpectedResult = true)]
        [TestCase("1.0.0-beta.11", "1.0.0-rc.1", ExpectedResult = true)]
        [TestCase("1.0.0-rc.1", "1.0.0", ExpectedResult = true)]
        [TestCase("1.0.0-alpha", "1.0.0-alpha", ExpectedResult = false)]
        [TestCase("1.0.0-alpha.1", "1.0.0-alpha", ExpectedResult = false)]
        public bool LessThan(string a, string b)
        {
            var versionA = Version.Create(a);
            var versionB = Version.Create(b);
            return versionA < versionB;
        }

        [TestCase("1.0.0-alpha", "1.0.0-alpha.1", ExpectedResult = true)]
        [TestCase("1.0.0-alpha", "1.0.0-alpha", ExpectedResult = true)]
        [TestCase("1.0.0-alpha.1", "1.0.0-alpha", ExpectedResult = false)]
        public bool LessThanEqual(string a, string b)
        {
            var versionA = Version.Create(a);
            var versionB = Version.Create(b);
            return versionA <= versionB;
        }

        [TestCase("1.0.0-alpha.1", "1.0.0-alpha", ExpectedResult = true)]
        [TestCase("1.0.0-alpha.beta", "1.0.0-alpha.1", ExpectedResult = true)]
        [TestCase("1.0.0-beta", "1.0.0-alpha.beta", ExpectedResult = true)]
        [TestCase("1.0.0-beta.2", "1.0.0-beta", ExpectedResult = true)]
        [TestCase("1.0.0-beta.11", "1.0.0-beta.2", ExpectedResult = true)]
        [TestCase("1.0.0-rc.1", "1.0.0-beta.11", ExpectedResult = true)]
        [TestCase("1.0.0", "1.0.0-rc.1", ExpectedResult = true)]
        [TestCase("1.0.0-alpha", "1.0.0-alpha", ExpectedResult = false)]
        [TestCase("1.0.0-alpha", "1.0.0-alpha.1", ExpectedResult = false)]
        public bool GreaterThan(string a, string b)
        {
            var versionA = Version.Create(a);
            var versionB = Version.Create(b);
            return versionA > versionB;
        }

        [TestCase("1.0.0-alpha.1", "1.0.0-alpha", ExpectedResult = true)]
        [TestCase("1.0.0-alpha", "1.0.0-alpha", ExpectedResult = true)]
        [TestCase("1.0.0-alpha", "1.0.0-alpha.1", ExpectedResult = false)]
        public bool GreaterThanEqual(string a, string b)
        {
            var versionA = Version.Create(a);
            var versionB = Version.Create(b);
            return versionA >= versionB;
        }
    }
}
