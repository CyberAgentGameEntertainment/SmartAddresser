using System;

namespace SmartAddresser.Editor.Core.Models.EntryRules.AddressRules
{
    /// <summary>
    ///     Provide addresses from asset information.
    /// </summary>
    public interface IAddressProvider
    {
        void Setup();

        string CreateAddress(string assetPath, Type assetType, bool isFolder);

        string GetDescription();
    }
}
