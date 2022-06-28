using System;

namespace SmartAddresser.Editor.Foundation.SemanticVersioning
{
    public sealed class VersionComparator
    {
        public enum Operator
        {
            Equal,
            NotEqual,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///     If <see cref="version" /> is 1.0.0 and <see cref="operatorType" /> is <see cref="Operator.LessThan" />,
        ///     checks that the argument of the <see cref="IsSatisfied" /> is less than 1.0.0.
        /// </remarks>
        /// <param name="version"></param>
        /// <param name="operatorType"></param>
        public VersionComparator(Version version, Operator operatorType)
        {
            Version = version;
            OperatorType = operatorType;
        }

        public Version Version { get; }

        public Operator OperatorType { get; }

        public bool IsSatisfied(Version version)
        {
            switch (OperatorType)
            {
                case Operator.Equal:
                    return version == Version;
                case Operator.NotEqual:
                    return version != Version;
                case Operator.LessThan:
                    return version < Version;
                case Operator.LessThanOrEqual:
                    return version <= Version;
                case Operator.GreaterThan:
                    return version > Version;
                case Operator.GreaterThanOrEqual:
                    return version >= Version;
                default:
                    throw new InvalidOperationException("Comparator type not recognised.");
            }
        }
    }
}
