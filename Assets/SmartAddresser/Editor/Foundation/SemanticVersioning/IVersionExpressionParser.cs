namespace SmartAddresser.Editor.Foundation.SemanticVersioning
{
    /// <summary>
    ///     Interface to parse version expression string to <see cref="CompositeVersionComparator" />.
    /// </summary>
    public interface IVersionExpressionParser
    {
        bool TryCreateComparator(string expression, out CompositeVersionComparator result);

        CompositeVersionComparator CreateComparator(string expression);
    }
}
