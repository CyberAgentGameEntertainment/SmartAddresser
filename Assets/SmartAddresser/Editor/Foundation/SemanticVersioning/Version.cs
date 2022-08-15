using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.SemanticVersioning
{
    /// <summary>
    ///     Version based on Semantic Versioning (https://semver.org/).
    /// </summary>
    public readonly struct Version : IComparable<Version>, IEquatable<Version>
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public string Prerelease { get; }
        public string Build { get; }

        private static readonly Regex VersionRegex = new Regex(@"^
            ([0-9]|[1-9][0-9]+) # Major
            \.
            ([0-9]|[1-9][0-9]+) # Minor
            \.
            ([0-9]|[1-9][0-9]+) # Patch
            (\-([0-9A-Za-z\-\.]+))? # Prerelease
            (\+([0-9A-Za-z\-\.]+))? # Build
            $",
            RegexOptions.IgnorePatternWhitespace);

        public static Version Create(string input)
        {
            return new Version(input);
        }

        public static bool TryCreate(string input, out Version result)
        {
            try
            {
                result = Create(input);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public Version(int major, int minor = 0, int patch = 0, string prerelease = null, string build = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Prerelease = prerelease ?? "";
            Build = build ?? "";
        }

        private Version(string input)
        {
            // If minor/patch versions are lacked, set them to zero.
            var inputs = input.Split('.').ToList();
            while (inputs.Count <= 2)
                inputs.Add("0");
            input = string.Join(".", inputs);

            var match = VersionRegex.Match(input);
            if (!match.Success)
                throw new ArgumentException($"Version string is invalid: {input}");

            Major = int.Parse(match.Groups[1].Value);
            Minor = int.Parse(match.Groups[2].Value);
            Patch = int.Parse(match.Groups[3].Value);
            Prerelease = match.Groups[4].Success ? match.Groups[5].Value : "";
            Build = match.Groups[6].Success ? match.Groups[7].Value : "";
        }

        public int CompareTo(Version other)
        {
            // Compare major, minor, patch version.
            var result = Major.CompareTo(other.Major);
            if (result != 0)
                return result;

            result = Minor.CompareTo(other.Minor);
            if (result != 0)
                return result;

            result = Patch.CompareTo(other.Patch);
            if (result != 0)
                return result;

            // Compare prerelease version.
            return ComparePrerelease(Prerelease, other.Prerelease);
        }

        private static int ComparePrerelease(string self, string other)
        {
            var selfIsNullOrEmpty = string.IsNullOrEmpty(self);
            var otherIsNullOrEmpty = string.IsNullOrEmpty(other);
            if (selfIsNullOrEmpty && otherIsNullOrEmpty)
                return 0;
            if (selfIsNullOrEmpty)
                return 1;
            if (otherIsNullOrEmpty)
                return -1;

            var selfParts = self.Split('.');
            var otherParts = other.Split('.');
            var minLength = Mathf.Min(selfParts.Length, otherParts.Length);
            for (var i = 0; i < minLength; i++)
            {
                var selfPart = selfParts[i];
                var otherPart = otherParts[i];
                var selfPartIsNumeric = int.TryParse(selfPart, out var selfNumber);
                var otherPartIsNumeric = int.TryParse(otherPart, out var otherNumber);

                int result;

                // If both are numeric, smaller value is smaller version.
                if (selfPartIsNumeric && otherPartIsNumeric)
                {
                    result = selfNumber.CompareTo(otherNumber);
                    if (result != 0)
                        return result;

                    continue;
                }

                // Numeric value is lower version than string.
                if (selfPartIsNumeric)
                    return -1;
                if (otherPartIsNumeric)
                    return 1;

                // Strings are compared in ASCII order.
                result = string.CompareOrdinal(selfPart, otherPart);
                if (result != 0)
                    return result;
            }

            return selfParts.Length.CompareTo(otherParts.Length);
        }

        public static int Compare(Version left, Version right)
        {
            return left.CompareTo(right);
        }

        public static bool operator ==(Version left, Version right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Version left, Version right)
        {
            return !Equals(left, right);
        }

        public static bool operator >(Version left, Version right)
        {
            return Compare(left, right) > 0;
        }

        public static bool operator >=(Version left, Version right)
        {
            return left == right || left > right;
        }

        public static bool operator <(Version left, Version right)
        {
            return Compare(left, right) < 0;
        }

        public static bool operator <=(Version left, Version right)
        {
            return left == right || left < right;
        }

        public bool Equals(Version other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch &&
                   string.Equals(Prerelease, other.Prerelease, StringComparison.Ordinal);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return other.GetType() == GetType() && Equals((Version)other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Patch;
                hashCode = (hashCode * 397) ^ (Prerelease != null ? Prerelease.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Build != null ? Build.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            var version = $"{Major}.{Minor}.{Patch}";
            if (!string.IsNullOrEmpty(Prerelease)) version += $"-{Prerelease}";
            if (!string.IsNullOrEmpty(Build)) version += $"+{Build}";

            return version;
        }
    }
}
