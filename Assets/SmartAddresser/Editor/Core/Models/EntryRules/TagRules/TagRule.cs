using System;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.EntryRules.TagRules
{
    /// <summary>
    ///     Provide rules for setting tags.
    /// </summary>
    [Serializable]
    public sealed class TagRule
    {
        [SerializeField] private string _id;
        [SerializeField] private AssetGroupObservableList _assetGroups = new AssetGroupObservableList();
        private readonly ObservableProperty<string> _assetGroupDescription = new ObservableProperty<string>();

        private readonly Subject<ITagProvider> _tagProviderChangedSubject = new Subject<ITagProvider>();
        private readonly ObservableProperty<string> _tagProviderDescription = new ObservableProperty<string>();

        [SerializeReference] private ITagProvider _tagProvider;

        public TagRule()
        {
            _id = IdentifierFactory.Create();
        }

        public string Id => _id;

        public IObservableList<AssetGroup> AssetGroups => _assetGroups;
        public IReadOnlyObservableProperty<string> AssetGroupDescription => _assetGroupDescription;
        public IReadOnlyObservableProperty<string> TagProviderDescription => _tagProviderDescription;

        public ITagProvider TagProvider
        {
            get => _tagProvider;
            set
            {
                if (_tagProvider != null && _tagProvider == value)
                    return;

                _tagProvider = value;
                _tagProviderChangedSubject.OnNext(value);
            }
        }

        public IObservable<ITagProvider> TagProviderChangedAsObservable => _tagProviderChangedSubject;

        /// <summary>
        ///     Setup to generate tags.
        ///     This method must be called before calling <see cref="CreateTag" />.
        /// </summary>
        public void Setup()
        {
            _assetGroups.Setup();
            _tagProvider.Setup();
        }

        /// <summary>
        ///     Create a tag from asset information.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="tag">If successful, assign the address. If not, null.</param>
        /// <returns>Return true if successful.</returns>
        public bool CreateTag(string assetPath, Type assetType, bool isFolder, out string tag)
        {
            if (!_assetGroups.Contains(assetPath, assetType, isFolder))
            {
                tag = null;
                return false;
            }

            tag = _tagProvider.Provide(assetPath, assetType, isFolder);
            return true;
        }

        internal void RefreshAssetGroupDescription()
        {
            var description = _assetGroups.GetDescription();
            if (string.IsNullOrEmpty(description))
                description = "(None)";
            _assetGroupDescription.Value = description;
        }

        internal void RefreshTagProviderDescription()
        {
            _tagProviderDescription.Value = _tagProvider == null ? "(None)" : _tagProvider.GetDescription();
        }
    }
}
