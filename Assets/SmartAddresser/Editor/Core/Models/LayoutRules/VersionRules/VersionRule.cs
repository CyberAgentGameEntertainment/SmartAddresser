using System;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    /// <summary>
    ///     Provide rules for setting versions.
    /// </summary>
    [Serializable]
    public sealed class VersionRule : ISerializationCallbackReceiver
    {
        [SerializeField] private string _id;
        [SerializeField] private ObservableProperty<string> _name = new ObservableProperty<string>("New Version Rule");
        [SerializeField] private AssetGroupObservableList _assetGroups = new AssetGroupObservableList();

        [SerializeReference] private IVersionProvider _versionProviderInternal;
        private ObservableProperty<string> _assetGroupDescription = new ObservableProperty<string>();
        private ObservableProperty<string> _versionProviderDescription = new ObservableProperty<string>();

        public VersionRule()
        {
            _id = IdentifierFactory.Create();
            var defaultAssetGroup = new AssetGroup
            {
                Name =
                {
                    Value = "Default Asset Group"
                }
            };
            _assetGroups.Add(defaultAssetGroup);
            VersionProvider.Value = new ConstantVersionProvider();
        }

        public string Id => _id;
        public IObservableProperty<string> Name => _name;
        public ObservableList<AssetGroup> AssetGroups => _assetGroups;
        public IReadOnlyObservableProperty<string> AssetGroupDescription => _assetGroupDescription;
        public IReadOnlyObservableProperty<string> VersionProviderDescription => _versionProviderDescription;

        public ObservableProperty<IVersionProvider> VersionProvider { get; set; } =
            new ObservableProperty<IVersionProvider>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _versionProviderInternal = VersionProvider.Value;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            VersionProvider.Value = _versionProviderInternal;
        }

        /// <summary>
        ///     Setup to generate versions.
        ///     This method must be called before calling <see cref="TryProvideVersion" />.
        /// </summary>
        public void Setup()
        {
            _assetGroups.Setup();
            VersionProvider.Value.Setup();
        }

        public bool Validate(out VersionRuleValidationError error)
        {
            if (_assetGroups.Validate(out var groupErrors))
            {
                error = null;
                return true;
            }

            error = new VersionRuleValidationError(this, groupErrors);
            return false;
        }

        /// <summary>
        ///     Create a version from asset information with addressable context.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="address">The address assigned to the addressable entry.</param>
        /// <param name="addressableAssetGroup">The addressable asset group.</param>
        /// <param name="version">If successful, assign the version. If not, null.</param>
        /// <param name="checkIsPathValidForEntry">
        ///     If true, check if the asset path is valid for entry.
        ///     You can pass false if it is guaranteed to be valid.
        /// </param>
        /// <returns>Return true if successful.</returns>
        public bool TryProvideVersion(
            string assetPath,
            Type assetType,
            bool isFolder,
            string address,
            AddressableAssetGroup addressableAssetGroup,
            out string version,
            bool checkIsPathValidForEntry = true
        )
        {
            if (!_assetGroups.Contains(assetPath, assetType, isFolder, address, addressableAssetGroup))
            {
                version = null;
                return false;
            }

            if (checkIsPathValidForEntry && !AddressableAssetUtility.IsAssetPathValidForEntry(assetPath))
            {
                version = null;
                return false;
            }

            version = VersionProvider.Value.Provide(assetPath, assetType, isFolder, address, addressableAssetGroup);

            if (string.IsNullOrEmpty(version))
            {
                version = null;
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

        internal void RefreshVersionProviderDescription()
        {
            var description = VersionProvider.Value?.GetDescription();
            if (string.IsNullOrEmpty(description))
                description = "(None)";
            _versionProviderDescription.Value = description;
        }
    }
}
