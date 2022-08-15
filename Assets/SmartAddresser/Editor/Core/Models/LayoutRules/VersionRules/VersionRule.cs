using System;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules
{
    /// <summary>
    ///     Provide rules for setting versions.
    /// </summary>
    [Serializable]
    public sealed class VersionRule
    {
        [SerializeField] private string _id;
        [SerializeField] private AssetGroupObservableList _assetGroups = new AssetGroupObservableList();
        private readonly ObservableProperty<string> _assetGroupDescription = new ObservableProperty<string>();

        private readonly Subject<IVersionProvider> _versionProviderChangedSubject = new Subject<IVersionProvider>();
        private readonly ObservableProperty<string> _versionProviderDescription = new ObservableProperty<string>();

        [SerializeReference] private IVersionProvider _versionProvider;

        public VersionRule()
        {
            _id = IdentifierFactory.Create();
        }

        public string Id => _id;

        public IObservableList<AssetGroup> AssetGroups => _assetGroups;
        public IReadOnlyObservableProperty<string> AssetGroupDescription => _assetGroupDescription;
        public IReadOnlyObservableProperty<string> VersionProviderDescription => _versionProviderDescription;

        public IVersionProvider VersionProvider
        {
            get => _versionProvider;
            set
            {
                if (_versionProvider != null && _versionProvider == value)
                    return;

                _versionProvider = value;
                _versionProviderChangedSubject.OnNext(value);
            }
        }

        public IObservable<IVersionProvider> VersionProviderChangedAsObservable => _versionProviderChangedSubject;

        /// <summary>
        ///     Setup to generate versions.
        ///     This method must be called before calling <see cref="TryProvideVersion" />.
        /// </summary>
        public void Setup()
        {
            _assetGroups.Setup();
            _versionProvider.Setup();
        }

        /// <summary>
        ///     Create a version from asset information.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="version">If successful, assign the address. If not, null.</param>
        /// <returns>Return true if successful.</returns>
        public bool TryProvideVersion(string assetPath, Type assetType, bool isFolder, out string version)
        {
            if (!_assetGroups.Contains(assetPath, assetType, isFolder))
            {
                version = null;
                return false;
            }

            version = _versionProvider.Provide(assetPath, assetType, isFolder);
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
            _versionProviderDescription.Value = _versionProvider == null ? "(None)" : _versionProvider.GetDescription();
        }
    }
}
