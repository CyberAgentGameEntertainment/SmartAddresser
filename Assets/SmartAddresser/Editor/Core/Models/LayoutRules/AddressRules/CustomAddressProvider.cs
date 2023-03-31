using System;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules
{
    [Serializable]
    public sealed class CustomAddressProvider : IAddressProvider
    {
        public AddressProviderAsset addressProvider;

        public void Setup()
        {
            if (addressProvider == null)
                return;
            
            addressProvider.Setup();
        }

        public string Provide(string assetPath, Type assetType, bool isFolder)
        {
            if (addressProvider == null)
                return null;
            
            return addressProvider.Provide(assetPath, assetType, isFolder);
        }

        public string GetDescription()
        {
            if (addressProvider == null)
                return string.Empty;
            
            return addressProvider.GetDescription();
        }
    }
}
