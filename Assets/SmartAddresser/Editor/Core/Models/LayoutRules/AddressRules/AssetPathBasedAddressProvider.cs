using System;
using SmartAddresser.Editor.Core.Models.Shared;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules
{
    /// <summary>
    ///     Provide addresses based on asset paths.
    /// </summary>
    [Serializable]
    public sealed class AssetPathBasedAddressProvider : AssetPathBasedProvider, IAddressProvider
    {
    }
}
