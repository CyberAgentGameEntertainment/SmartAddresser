using System;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    /// <summary>
    ///     Provide rules for setting labels.
    /// </summary>
    [Serializable]
    public sealed class LabelRule
    {
        [SerializeField] private string _id;
        [SerializeField] private AssetGroupObservableList _assetGroups = new AssetGroupObservableList();
        private readonly ObservableProperty<string> _assetGroupDescription = new ObservableProperty<string>();

        private readonly Subject<ILabelProvider> _labelProviderChangedSubject = new Subject<ILabelProvider>();
        private readonly ObservableProperty<string> _labelProviderDescription = new ObservableProperty<string>();

        [SerializeReference] private ILabelProvider _labelProvider;

        public LabelRule()
        {
            _id = IdentifierFactory.Create();
        }

        public string Id => _id;

        public IObservableList<AssetGroup> AssetGroups => _assetGroups;
        public IReadOnlyObservableProperty<string> AssetGroupDescription => _assetGroupDescription;
        public IReadOnlyObservableProperty<string> LabelProviderDescription => _labelProviderDescription;

        public ILabelProvider LabelProvider
        {
            get => _labelProvider;
            set
            {
                if (_labelProvider != null && _labelProvider == value)
                    return;

                _labelProvider = value;
                _labelProviderChangedSubject.OnNext(value);
            }
        }

        public IObservable<ILabelProvider> LabelProviderChangedAsObservable => _labelProviderChangedSubject;

        /// <summary>
        ///     Setup to generate labels.
        ///     This method must be called before calling <see cref="CreateLabel" />.
        /// </summary>
        public void Setup()
        {
            _assetGroups.Setup();
            _labelProvider.Setup();
        }

        /// <summary>
        ///     Create a label from asset information.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="label">If successful, assign the address. If not, null.</param>
        /// <returns>Return true if successful.</returns>
        public bool CreateLabel(string assetPath, Type assetType, bool isFolder, out string label)
        {
            if (!_assetGroups.Contains(assetPath, assetType, isFolder))
            {
                label = null;
                return false;
            }

            label = _labelProvider.Provide(assetPath, assetType, isFolder);
            return true;
        }

        internal void RefreshAssetGroupDescription()
        {
            var description = _assetGroups.GetDescription();
            if (string.IsNullOrEmpty(description))
                description = "(None)";
            _assetGroupDescription.Value = description;
        }

        internal void RefreshLabelProviderDescription()
        {
            _labelProviderDescription.Value = _labelProvider == null ? "(None)" : _labelProvider.GetDescription();
        }
    }
}
