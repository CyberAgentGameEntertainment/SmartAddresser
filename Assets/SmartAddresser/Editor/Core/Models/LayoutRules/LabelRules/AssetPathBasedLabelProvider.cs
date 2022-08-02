using System;
using SmartAddresser.Editor.Core.Models.Shared;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    /// <summary>
    ///     Provide the label based on asset paths.
    /// </summary>
    [Serializable]
    public sealed class AssetPathBasedLabelProvider : AssetPathBasedProvider, ILabelProvider
    {
    }
}
