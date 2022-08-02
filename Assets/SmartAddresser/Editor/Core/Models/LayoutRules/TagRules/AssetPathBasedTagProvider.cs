using System;
using SmartAddresser.Editor.Core.Models.Shared;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.TagRules
{
    /// <summary>
    ///     Provide the tag based on asset paths.
    /// </summary>
    [Serializable]
    public sealed class AssetPathBasedTagProvider : AssetPathBasedProvider, ITagProvider
    {
    }
}
