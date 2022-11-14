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
        [SerializeField] private ObservableProperty<string> _name = new ObservableProperty<string>("New Label Rule");
        [SerializeField] private AssetGroupObservableList _assetGroups = new AssetGroupObservableList();

        private ObservableProperty<string> _assetGroupDescription = new ObservableProperty<string>();
        private ObservableProperty<string> _labelProviderDescription = new ObservableProperty<string>();

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
        public IObservableProperty<string> Name => _name;
        public ObservableList<AssetGroup> AssetGroups => _assetGroups;
        public IReadOnlyObservableProperty<string> AssetGroupDescription => _assetGroupDescription;
        public IReadOnlyObservableProperty<string> LabelProviderDescription => _labelProviderDescription;

        public ObservableProperty<ILabelProvider> LabelProvider { get; set; } =
            new ObservableProperty<ILabelProvider>();

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
            LabelProvider.Value.Setup();
        }

        /// <summary>
        ///     Create a label from asset information.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="label">If successful, assign the address. If not, null.</param>
        /// <param name="checkIsPathValidForEntry">
        ///     If true, check if the asset path is valid for entry.
        ///     You can pass false if it is guaranteed to be valid.
        /// </param>
        /// <returns>Return true if successful.</returns>
        public bool TryProvideLabel(string assetPath, Type assetType, bool isFolder, out string label,
            bool checkIsPathValidForEntry = true)
        {
            if (!_assetGroups.Contains(assetPath, assetType, isFolder))
            {
                label = null;
                return false;
            }

            if (checkIsPathValidForEntry && !AddressableAssetUtility.IsAssetPathValidForEntry(assetPath))
            {
                label = null;
                return false;
            }

            label = LabelProvider.Value.Provide(assetPath, assetType, isFolder);

            if (string.IsNullOrEmpty(label))
            {
                label = null;
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

        internal void RefreshLabelProviderDescription()
        {
            var description = LabelProvider.Value?.GetDescription();
            if (string.IsNullOrEmpty(description))
                description = "(None)";
            _labelProviderDescription.Value = description;
        }
    }
}
