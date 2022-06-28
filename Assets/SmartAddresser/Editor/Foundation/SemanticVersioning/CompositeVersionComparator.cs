using System.Collections.Generic;

namespace SmartAddresser.Editor.Foundation.SemanticVersioning
{
    /// <summary>
    ///     Class that handle multiple <see cref="VersionComparator" />s transparently.
    /// </summary>
    public sealed class CompositeVersionComparator
    {
        private readonly List<VersionComparator> _comparators = new List<VersionComparator>();

        public IReadOnlyList<VersionComparator> Comparators => _comparators;

        public void Add(VersionComparator comparator)
        {
            _comparators.Add(comparator);
        }

        public bool IsSatisfied(Version version)
        {
            if (Comparators.Count == 0)
                return false;

            foreach (var comparator in _comparators)
                if (!comparator.IsSatisfied(version))
                    return false;

            return true;
        }
    }
}
