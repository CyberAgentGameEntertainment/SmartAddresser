using System;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.EntryRules.AddressRules
{
    /// <summary>
    ///     Provide rules for setting addresses.
    /// </summary>
    [Serializable]
    public sealed class AddressRule
    {
        [SerializeField] private string _id;
        [SerializeField] private AssetGroupCollection _assetGroups = new AssetGroupCollection();
        [SerializeReference] private IAddressProvider _addressProvider;

        private Subject<IAddressProvider> _addressProviderChangedSubject = new Subject<IAddressProvider>();

        public AddressRule()
        {
            _id = IdentifierFactory.Create();
        }

        public string Id => _id;
        public AssetGroupCollection AssetGroups => _assetGroups;
        public IObservable<IAddressProvider> AddressProviderChangedAsObservable => _addressProviderChangedSubject;

        public IAddressProvider AddressProvider
        {
            get => _addressProvider;
            set => _addressProvider = value;
        }

        /// <summary>
        ///     Setup to generate addresses.
        ///     This method must be called before calling <see cref="CreateAddress" />.
        /// </summary>
        public void Setup()
        {
            _assetGroups.Setup();
            _addressProvider.Setup();
        }

        /// <summary>
        ///     Create an address from asset information.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="address">If successful, assign the address. If not, null.</param>
        /// <returns>Return true if successful.</returns>
        public bool CreateAddress(string assetPath, Type assetType, bool isFolder, out string address)
        {
            if (!_assetGroups.IsTargetAsset(assetPath, assetType, isFolder))
            {
                address = null;
                return false;
            }

            address = _addressProvider.CreateAddress(assetPath, assetType, isFolder);
            return true;
        }
    }
}
