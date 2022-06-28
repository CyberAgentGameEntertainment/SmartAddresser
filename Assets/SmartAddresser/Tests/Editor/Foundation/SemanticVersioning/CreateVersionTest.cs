using NUnit.Framework;
using SmartAddresser.Editor.Foundation.SemanticVersioning;

namespace SmartAddresser.Tests.Editor.Foundation.SemanticVersioning
{
    internal sealed class CreateVersionTest
    {
        [TestCase("0", 0, 0, 0, "", "")]
        [TestCase("1.0", 1, 0, 0, "", "")]
        [TestCase("1.12.13-prev01", 1, 12, 13, "prev01", "")]
        [TestCase("13.2.300-alpha+b1", 13, 2, 300, "alpha", "b1")]
        public void Create(string versionString, int major, int minor, int patch, string prerelease, string build)
        {
            var version = Version.Create(versionString);
            Assert.That(version.Major, Is.EqualTo(major));
            Assert.That(version.Minor, Is.EqualTo(minor));
            Assert.That(version.Patch, Is.EqualTo(patch));
            Assert.That(version.Prerelease, Is.EqualTo(prerelease));
            Assert.That(version.Build, Is.EqualTo(build));
        }

        [TestCase("13", ExpectedResult = true)]
        [TestCase("13.2", ExpectedResult = true)]
        [TestCase("0.0.3", ExpectedResult = true)]
        [TestCase("1.12.13-prev01", ExpectedResult = true)]
        [TestCase("13.2.300-alpha+b1", ExpectedResult = true)]
        [TestCase("13.02.300", ExpectedResult = false)]
        [TestCase("13.2.03", ExpectedResult = false)]
        [TestCase("original-version-string", ExpectedResult = false)]
        public bool TryCreate(string versionString)
        {
            return Version.TryCreate(versionString, out _);
        }
    }
}
