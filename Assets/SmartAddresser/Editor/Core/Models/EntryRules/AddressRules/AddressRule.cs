using System;
using System.Collections.Generic;
using System.Text;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor.AddressableAssets.Settings;
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
        [SerializeField] private AddressableAssetGroup _addressableGroup;
        [SerializeField] private bool _control;
        [SerializeField] private ObservableList<AssetGroup> _assetGroups = new ObservableList<AssetGroup>();

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
        ///     This method must be called before calling <see cref="CreateAddress" />.
        /// </summary>
        public void Setup()
        {
            foreach (var group in _assetGroups)
                group.Setup();
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
            for (var i = 0; i < _assetGroups.Count; i++)
            {
                if (!_assetGroups[i].Contains(assetPath, assetType, isFolder))
                    continue;

                address = _addressProvider.CreateAddress(assetPath, assetType, isFolder);
                return true;
            }

            address = null;
            return false;
        }

        internal void RefreshAssetGroupDescription()
        {
            var groupDescriptions = new List<string>(_assetGroups.Count);

            foreach (var group in _assetGroups)
            {
                var groupDescription = group.GetDescription();

                if (string.IsNullOrEmpty(groupDescription))
                    continue;

                groupDescriptions.Add(groupDescription);
            }

            if (groupDescriptions.Count == 0)
            {
                _assetGroupDescription.Value = "(None)";
                return;
            }

            var description = new StringBuilder();
            for (var i = 0; i < groupDescriptions.Count; i++)
            {
                var groupDescription = groupDescriptions[i];

                if (i >= 1)
                    description.Append(" || ");

                if (groupDescriptions.Count >= 2)
                    description.Append(" (");

                description.Append(groupDescription);

                if (groupDescriptions.Count >= 2)
                    description.Append(") ");
            }

            _assetGroupDescription.Value = description.ToString();
        }

        internal void RefreshAddressProviderDescription()
        {
            _addressProviderDescription.Value = _addressProvider == null ? "(None)" : _addressProvider.GetDescription();
        }
    }
}
