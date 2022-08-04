using System;
using SmartAddresser.Editor.Core.Models.Shared;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    /// <summary>
    ///     Provide the tag based on asset paths.
    /// </summary>
    [Serializable]
    public sealed class AssetPathBasedVersionProvider : AssetPathBasedProvider, IVersionProvider
    {
    }
}
