using System;
using System.Text.RegularExpressions;

namespace SmartAddresser.Runtime.Foundation.SemanticVersioning
{
    /// <summary>
    ///     <see cref="IVersionExpressionParser" /> implementation like Unity's Version Define expressions
    ///     (https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html).
    /// </summary>
    public sealed class UnityVersionExpressionParser : IVersionExpressionParser
    {
        public bool TryCreateComparator(string expression, out CompositeVersionComparator result)
        {
            try
            {
                result = CreateComparator(expression);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public CompositeVersionComparator CreateComparator(string expression)
        {
            var result = new CompositeVersionComparator();
            var split = expression.Split(',');
            if (split.Length == 1)
            {
                var regex = new Regex(@"^\[(.+)\]");
                var match = regex.Match(expression);
                if (match.Success)
                {
                    // "[2.4.5]" is "x = 2.4.5" 
                    var versionStr = match.Groups[1].Value;
                    if (Version.TryCreate(versionStr, out var version))
                        result.Add(new VersionComparator(version, VersionComparator.Operator.Equal));
                    else
                        throw new ArgumentException($"Invalid format expression: {expression}");
                }
                else
                {
                    // "2.1.0-preview.7" is "x >= 2.1.0-preview.7"
                    if (Version.TryCreate(expression, out var version))
                        result.Add(new VersionComparator(version, VersionComparator.Operator.GreaterThanOrEqual));
                    else
                        throw new ArgumentException($"Invalid format expression: {expression}");
                }
            }
            else if (split.Length == 2)
            {
                // "[1.3,3.4.1]" is "1.3.0 <= x <= 3.4.1"
                // "(1.3.0,3.4)" is "1.3.0 < x < 3.4.0"
                // "[1.1,3.4)" is "1.1.0 <= x < 3.4.0"
                // "(0.2.4,5.6.2-preview.2]" is "0.2.4 < x <= 5.6.2.-preview.2"
                var firstChar = expression.Substring(0, 1);
                var minVersionStr = split[0].Substring(1, split[0].Length - 1);
                if (firstChar == "[")
                {
                    if (Version.TryCreate(minVersionStr, out var version))
                        result.Add(new VersionComparator(version, VersionComparator.Operator.GreaterThanOrEqual));
                    else
                        throw new ArgumentException($"Invalid format expression: {expression}");
                }
                else if (firstChar == "(")
                {
                    if (Version.TryCreate(minVersionStr, out var version))
                        result.Add(new VersionComparator(version, VersionComparator.Operator.GreaterThan));
                    else
                        throw new ArgumentException($"Invalid format expression: {expression}");
                }
                else
                {
                    throw new ArgumentException($"Invalid format expression: {expression}");
                }

                var lastChar = expression.Substring(expression.Length - 1, 1);
                var maxVersionStr = split[1].Substring(0, split[1].Length - 1);
                if (lastChar == "]")
                {
                    if (Version.TryCreate(maxVersionStr, out var version))
                        result.Add(new VersionComparator(version, VersionComparator.Operator.LessThanOrEqual));
                    else
                        throw new ArgumentException($"Invalid format expression: {expression}");
                }
                else if (lastChar == ")")
                {
                    if (Version.TryCreate(maxVersionStr, out var version))
                        result.Add(new VersionComparator(version, VersionComparator.Operator.LessThan));
                    else
                        throw new ArgumentException($"Invalid format expression: {expression}");
                }
                else
                {
                    throw new ArgumentException($"Invalid format expression: {expression}");
                }
            }
            else
            {
                throw new ArgumentException($"Invalid format expression: {expression}");
            }

            return result;
        }
    }
}
