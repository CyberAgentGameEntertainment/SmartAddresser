using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    [Serializable]
    public sealed class LayoutRule
    {
        [SerializeField] private ObservableList<AddressRule> _addressRules = new ObservableList<AddressRule>();
        [SerializeField] private ObservableList<LabelRule> _labelRules = new ObservableList<LabelRule>();
        [SerializeField] private ObservableList<VersionRule> _versionRules = new ObservableList<VersionRule>();

        public IObservableList<AddressRule> AddressRules => _addressRules;
        public IObservableList<LabelRule> LabelRules => _labelRules;
        public IObservableList<VersionRule> VersionRules => _versionRules;

        public void SetupForAddress()
        {
            foreach (var addressRule in _addressRules)
                addressRule.Setup();
        }

        public bool ProvideAddressAndAddressableGroup(string assetPath, Type assetType, bool isFolder, bool doSetup,
            out string address, out AddressableAssetGroup addressableGroup)
        {
            foreach (var addressRule in _addressRules)
            {
                if (doSetup)
                    addressRule.Setup();

                // Adopt the first matching address.
                if (addressRule.TryProvideAddress(assetPath, assetType, isFolder, out address))
                {
                    addressableGroup = addressRule.AddressableGroup;
                    return true;
                }
            }

            address = null;
            addressableGroup = null;
            return false;
        }

        public void SetupForLabels()
        {
            foreach (var labelRule in _labelRules)
                labelRule.Setup();
        }

        public IReadOnlyCollection<string> ProvideLabels(string assetPath, Type assetType, bool isFolder, bool doSetup)
        {
            var labels = new HashSet<string>();
            foreach (var labelRule in _labelRules)
            {
                if (doSetup)
                    labelRule.Setup();

                if (labelRule.TryProvideLabel(assetPath, assetType, isFolder, out var label))
                    labels.Add(label);
            }

            return labels;
        }

        public void SetupForVersion()
        {
            foreach (var versionRule in _versionRules)
                versionRule.Setup();
        }

        public string ProvideVersion(string assetPath, Type assetType, bool isFolder, bool doSetup)
        {
            foreach (var versionRule in _versionRules)
            {
                if (doSetup)
                    versionRule.Setup();

                // Adopt the first matching version.
                if (versionRule.TryProvideVersion(assetPath, assetType, isFolder, out var version))
                    return version;
            }

            return null;
        }
    }
}
