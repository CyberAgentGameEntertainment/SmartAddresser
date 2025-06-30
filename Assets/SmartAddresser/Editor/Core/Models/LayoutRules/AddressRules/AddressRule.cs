using System;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
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
    public sealed class AddressRule : ISerializationCallbackReceiver
    {
        [SerializeField] private string _id;
        [SerializeField] private AddressableAssetGroup _addressableGroup;
        [SerializeField] private ObservableProperty<bool> _control = new ObservableProperty<bool>();
        [SerializeField] private AssetGroupObservableList _assetGroups = new AssetGroupObservableList();

        [SerializeReference] private IAddressProvider _addressProviderInternal;

        private ObservableProperty<string> _addressProviderDescription = new ObservableProperty<string>();
        private ObservableProperty<string> _assetGroupDescription = new ObservableProperty<string>();

        // Define the default constructor for serialization.
        private AddressRule()
        {
        }

        public AddressRule(AddressableAssetGroup addressableGroup)
        {
            _id = IdentifierFactory.Create();
            _addressableGroup = addressableGroup;
            var defaultAssetGroup = new AssetGroup
            {
                Name =
                {
                    Value = "Default Asset Group"
                }
            };
            _assetGroups.Add(defaultAssetGroup);
            AddressProvider.Value = new AssetPathBasedAddressProvider();
        }

        public ObservableProperty<IAddressProvider> AddressProvider { get; } =
            new ObservableProperty<IAddressProvider>();

        public AddressableAssetGroup AddressableGroup => _addressableGroup;

        public string Id => _id;
        public ObservableList<AssetGroup> AssetGroups => _assetGroups;
        public IReadOnlyObservableProperty<string> AssetGroupDescription => _assetGroupDescription;
        public IReadOnlyObservableProperty<string> AddressProviderDescription => _addressProviderDescription;

        public IObservableProperty<bool> Control => _control;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _addressProviderInternal = AddressProvider.Value;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            AddressProvider.Value = _addressProviderInternal;
        }

        /// <summary>
        ///     Setup to generate addresses.
        ///     This method must be called before calling <see cref="TryProvideAddress" />.
        /// </summary>
        public void Setup()
        {
            _assetGroups.Setup();
            AddressProvider.Value.Setup();
        }

        public bool Validate(out AddressRuleValidationError error)
        {
            if (!_control.Value)
            {
                error = null;
                return true;
            }

            if (_assetGroups.Validate(out var groupErrors))
            {
                error = null;
                return true;
            }

            error = new AddressRuleValidationError(this, groupErrors);
            return false;
        }

        /// <summary>
        ///     Provide an address from asset information.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="address">If successful, assign the address. If not, null.</param>
        /// <param name="checkIsPathValidForEntry">
        ///     If true, check if the asset path is valid for entry.
        ///     You can pass false if it is guaranteed to be valid.
        /// </param>
        /// <returns>Return true if successful.</returns>
        public bool TryProvideAddress(
            string assetPath,
            Type assetType,
            bool isFolder,
            out string address,
            bool checkIsPathValidForEntry = true
        )
        {
            // If this addressable group is not the control target, do nothing.
            if (!Control.Value)
            {
                address = null;
                return false;
            }

            if (!_assetGroups.Contains(assetPath, assetType, isFolder, null, null))
            {
                address = null;
                return false;
            }

            if (checkIsPathValidForEntry && !AddressableAssetUtility.IsAssetPathValidForEntry(assetPath))
            {
                address = null;
                return false;
            }

            address = AddressProvider.Value.Provide(assetPath, assetType, isFolder);

            if (string.IsNullOrEmpty(address))
            {
                address = null;
                return false;
            }

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
            var description = AddressProvider.Value?.GetDescription();
            if (string.IsNullOrEmpty(description))
                description = "(None)";
            _addressProviderDescription.Value = description;
        }
    }
}
