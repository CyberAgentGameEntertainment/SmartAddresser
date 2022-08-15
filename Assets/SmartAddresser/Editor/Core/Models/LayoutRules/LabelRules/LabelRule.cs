using System;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    /// <summary>
    ///     Provide rules for setting labels.
    /// </summary>
    [Serializable]
    public sealed class LabelRule : ISerializationCallbackReceiver
    {
        [SerializeField] private string _id;
        [SerializeField] private AssetGroupObservableList _assetGroups = new AssetGroupObservableList();
        private readonly ObservableProperty<string> _assetGroupDescription = new ObservableProperty<string>();

        private readonly ObservableProperty<string> _labelProviderDescription = new ObservableProperty<string>();

        [SerializeReference] private ILabelProvider _labelProviderInternal;

        public LabelRule()
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
            LabelProvider.Value = new ConstantLabelProvider();
        }

        public string Id => _id;

        public IObservableList<AssetGroup> AssetGroups => _assetGroups;
        public IReadOnlyObservableProperty<string> AssetGroupDescription => _assetGroupDescription;
        public IReadOnlyObservableProperty<string> LabelProviderDescription => _labelProviderDescription;

        public ObservableProperty<ILabelProvider> LabelProvider { get; } = new ObservableProperty<ILabelProvider>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _labelProviderInternal = LabelProvider.Value;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            LabelProvider.Value = _labelProviderInternal;
        }

        /// <summary>
        ///     Setup to generate labels.
        ///     This method must be called before calling <see cref="TryProvideLabel" />.
        /// </summary>
        public void Setup()
        {
            _assetGroups.Setup();
            _labelProviderInternal.Setup();
        }

        /// <summary>
        ///     Create a label from asset information.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="label">If successful, assign the address. If not, null.</param>
        /// <returns>Return true if successful.</returns>
        public bool TryProvideLabel(string assetPath, Type assetType, bool isFolder, out string label)
        {
            if (!_assetGroups.Contains(assetPath, assetType, isFolder))
            {
                label = null;
                return false;
            }

            label = _labelProviderInternal.Provide(assetPath, assetType, isFolder);
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
            _labelProviderDescription.Value =
                _labelProviderInternal == null ? "(None)" : _labelProviderInternal.GetDescription();
        }
    }
}
