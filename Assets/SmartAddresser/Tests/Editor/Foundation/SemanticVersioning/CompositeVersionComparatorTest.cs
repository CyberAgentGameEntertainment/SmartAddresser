using NUnit.Framework;
using SmartAddresser.Editor.Foundation.SemanticVersioning;

namespace SmartAddresser.Tests.Editor.Foundation.SemanticVersioning
{
    internal sealed class CompositeVersionComparatorTest
    {
        [TestCase("0.9.9", ExpectedResult = false)]
        [TestCase("1.0.0", ExpectedResult = false)]
        [TestCase("1.0.1", ExpectedResult = true)]
        [TestCase("1.2.3", ExpectedResult = true)]
        [TestCase("1.2.4", ExpectedResult = false)]
        public bool IsSatisfied(string targetVersionStr)
        {
            var minComparator = new VersionComparator(Version.Create("1.0.0"), VersionComparator.Operator.GreaterThan);
            var maxComparator =
                new VersionComparator(Version.Create("1.2.3"), VersionComparator.Operator.LessThanOrEqual);
            var compositeComparator = new CompositeVersionComparator();
            compositeComparator.Add(minComparator);
            compositeComparator.Add(maxComparator);

            var targetVersion = Version.Create(targetVersionStr);
            return compositeComparator.IsSatisfied(targetVersion);
        }
    }
}
