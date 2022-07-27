using System;

namespace SmartAddresser.Editor.Core.Models.Layouts
{
    /// <summary>
    ///     Error type for <see cref="Layout" />, <see cref="Group" /> and <see cref="Entry" />.
    ///     This is used to determine if the layout can be applied to Addressable Asset System.
    /// </summary>
    public enum LayoutErrorType
    {
        /// <summary>
        ///     No errors.
        ///     The layout van be applied to Addressable Asset System.
        /// </summary>
        None,

        /// <summary>
        ///     Warning.
        ///     The layout should not be applied to Addressable Asset System, but can.
        /// </summary>
        Warning,

        /// <summary>
        ///     Error.
        ///     The layout cannot be applied to Addressable Asset System.
        /// </summary>
        Error
    }

    public static class LayoutErrorTypeExtensions
    {
        public static bool IsMoreCriticalThan(this LayoutErrorType self, LayoutErrorType other)
        {
            return self switch
            {
                LayoutErrorType.None => false,
                LayoutErrorType.Warning => other == LayoutErrorType.None,
                LayoutErrorType.Error => other == LayoutErrorType.None || other == LayoutErrorType.Warning,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
