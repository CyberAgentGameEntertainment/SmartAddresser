using System;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules
{
    /// <summary>
    ///     Provide rules for setting addresses.
    /// </summary>
    [Serializable]
    public sealed class AddressRule
    {
        [SerializeField] private string _id;
        [SerializeField] private AddressableAssetGroup _addressableGroup;
        [SerializeField] private bool _control;
        [SerializeField] private AssetGroupObservableList _assetGroups = new AssetGroupObservableList();

        private readonly Subject<IAddressProvider> _addressProviderChangedSubject = new Subject<IAddressProvider>();
        private readonly ObservableProperty<string> _addressProviderDescription = new ObservableProperty<string>();
        private readonly ObservableProperty<string> _assetGroupDescription = new ObservableProperty<string>();

        [SerializeReference] private IAddressProvider _addressProvider;

        public AddressRule(AddressableAssetGroup addressableGroup)
        {
            _id = IdentifierFactory.Create();
            _addressableGroup = addressableGroup;
        }

        public AddressableAssetGroup AddressableGroup => _addressableGroup;

        public string Id => _id;

        public bool Control
        {
            get => _control;
            set => _control = value;
        }

        public IObservableList<AssetGroup> AssetGroups => _assetGroups;
        public IReadOnlyObservableProperty<string> AssetGroupDescription => _assetGroupDescription;
        public IReadOnlyObservableProperty<string> AddressProviderDescription => _addressProviderDescription;

        public IAddressProvider AddressProvider
        {
            get => _addressProvider;
            set
            {
                if (_addressProvider != null && _addressProvider == value)
                    return;

                _addressProvider = value;
                _addressProviderChangedSubject.OnNext(value);
            }
        }

        public IObservable<IAddressProvider> AddressProviderChangedAsObservable => _addressProviderChangedSubject;

        /// <summary>
        ///     Setup to generate addresses.
        ///     This method must be called before calling <see cref="TryProvideAddress" />.
        /// </summary>
        public void Setup()
        {
            _assetGroups.Setup();
            _addressProvider.Setup();
        }

        /// <summary>
        ///     Provide an address from asset information.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="address">If successful, assign the address. If not, null.</param>
        /// <returns>Return true if successful.</returns>
        public bool TryProvideAddress(string assetPath, Type assetType, bool isFolder, out string address)
        {
            if (!_assetGroups.Contains(assetPath, assetType, isFolder))
            {
                address = null;
                return false;
            }

            address = _addressProvider.Provide(assetPath, assetType, isFolder);
            return true;
        }

        internal void RefreshAssetGroupDescription()
        {
            var description = _assetGroups.GetDescription();
            if (string.IsNullOrEmpty(description))
                description = "(None)";
            _assetGroupDescription.Value = description;
        }

        internal void RefreshAddressProviderDescription()
        {
            _addressProviderDescription.Value = _addressProvider == null ? "(None)" : _addressProvider.GetDescription();
        }
    }
}
