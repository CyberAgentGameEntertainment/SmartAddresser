using System;
using SmartAddresser.Editor.Core.Models.Shared;

namespace SmartAddresser.Editor.Core.Models.EntryRules.AddressRules
{
    /// <summary>
    ///     Provide addresses from asset information.
    /// </summary>
    public interface IAddressProvider : IProvider<string>
    {
    }
}
