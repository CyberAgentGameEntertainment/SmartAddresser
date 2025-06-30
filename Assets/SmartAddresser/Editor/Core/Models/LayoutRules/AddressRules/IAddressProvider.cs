using System;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules
{
    /// <summary>
    ///     Provide addresses from asset information.
    /// </summary>
    public interface IAddressProvider
    {
        void Setup();

        string Provide(string assetPath, Type assetType, bool isFolder);

        string GetDescription();
    }
}
