using System;
using NUnit.Framework;
using SmartAddresser.Editor.Foundation.SemanticVersioning;
using Version = SmartAddresser.Editor.Foundation.SemanticVersioning.Version;

namespace SmartAddresser.Tests.Editor.Foundation.SemanticVersioning
{
    internal sealed class UnityVersionExpressionParserTest
    {
        [TestCase("[2.4.5]", "2.4.5", VersionComparator.Operator.Equal)]
        [TestCase("2.1.0-preview.7", "2.1.0-preview.7", VersionComparator.Operator.GreaterThanOrEqual)]
        public void SingleVersionExpression(string expression, string version, VersionComparator.Operator @operator)
        {
            var compositeComparator = new UnityVersionExpressionParser().CreateComparator(expression);

            Assert.That(compositeComparator.Comparators[0].Version, Is.EqualTo(Version.Create(version)));
            Assert.That(compositeComparator.Comparators[0].OperatorType, Is.EqualTo(@operator));
        }

        [TestCase("[1.3,3.4.1]", "1.3.0", VersionComparator.Operator.GreaterThanOrEqual, "3.4.1",
            VersionComparator.Operator.LessThanOrEqual)]
        [TestCase("(1.3.0,3.4)", "1.3.0", VersionComparator.Operator.GreaterThan, "3.4.0",
            VersionComparator.Operator.LessThan)]
        [TestCase("[1.1,3.4)", "1.1.0", VersionComparator.Operator.GreaterThanOrEqual, "3.4.0",
            VersionComparator.Operator.LessThan)]
        [TestCase("(0.2.4,5.6.2-preview.2]", "0.2.4", VersionComparator.Operator.GreaterThan, "5.6.2-preview.2",
            VersionComparator.Operator.LessThanOrEqual)]
        public void DoubleVersionExpression(string expression, string minVersion,
            VersionComparator.Operator minOperator, string maxVersion, VersionComparator.Operator maxOperator)
        {
            var compositeComparator = new UnityVersionExpressionParser().CreateComparator(expression);

            Assert.That(compositeComparator.Comparators[0].Version, Is.EqualTo(Version.Create(minVersion)));
            Assert.That(compositeComparator.Comparators[0].OperatorType, Is.EqualTo(minOperator));
            Assert.That(compositeComparator.Comparators[1].Version, Is.EqualTo(Version.Create(maxVersion)));
            Assert.That(compositeComparator.Comparators[1].OperatorType, Is.EqualTo(maxOperator));
        }

        [TestCase("[2.4.5")]
        [TestCase("(2.4.5)")]
        [TestCase("[ 2.4.5]")] // Space is not allowed.
        [TestCase("[2.*]")] // Wildcard is not allowed.
        [TestCase("[1.3.0, 3.4.1]")] // Space is not allowed.
        [TestCase("[1.3.0,3.*]")] // Wildcard is not allowed.
        public void InvalidVersionExpression(string expression)
        {
            Assert.Throws<ArgumentException>(() => new UnityVersionExpressionParser().CreateComparator(expression));
        }
    }
}
