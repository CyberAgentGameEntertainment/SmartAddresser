using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.Settings;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules
{
    [Serializable]
    public sealed class LayoutRule
    {
        [SerializeField] private LayoutRuleSettings _settings = new LayoutRuleSettings();
        [SerializeField] private ObservableList<AddressRule> _addressRules = new ObservableList<AddressRule>();
        [SerializeField] private ObservableList<LabelRule> _labelRules = new ObservableList<LabelRule>();
        [SerializeField] private ObservableList<VersionRule> _versionRules = new ObservableList<VersionRule>();

        public LayoutRuleSettings Settings => _settings;
        public IObservableList<AddressRule> AddressRules => _addressRules;
        public IObservableList<LabelRule> LabelRules => _labelRules;
        public IObservableList<VersionRule> VersionRules => _versionRules;

        public void Setup()
        {
            foreach (var addressRule in _addressRules)
                addressRule.Setup();
            foreach (var labelRule in _labelRules)
                labelRule.Setup();
            foreach (var versionRule in _versionRules)
                versionRule.Setup();
        }

        /// <summary>
        ///     <para>* If there is no address group that hold the addressable group, add it.</para>
        ///     <para>* Remove address rules that hold addressable groups that no longer exists.</para>
        ///     <para>* Order address group by addressable group.</para>
        /// </summary>
        /// <param name="addressableGroups"></param>
        /// <returns>If true, address rules have been changed.</returns>
        public bool SyncAddressRulesWithAddressableAssetGroups(List<AddressableAssetGroup> addressableGroups)
        {
            var isDirty = false;
            if (addressableGroups.Count != _addressRules.Count)
                isDirty = true;
            else
                for (var i = 0; i < addressableGroups.Count; i++)
                {
                    var addressableGroup = addressableGroups[i];
                    var addressRule = _addressRules[i];
                    if (addressRule.AddressableGroup == null || addressRule.AddressableGroup != addressableGroup)
                    {
                        isDirty = true;
                        break;
                    }
                }

            if (!isDirty)
                return false;

            var newList = new List<AddressRule>();
            foreach (var addressableGroup in addressableGroups)
            {
                var addressRule = _addressRules.FirstOrDefault(x => x.AddressableGroup == addressableGroup)
                                  ?? new AddressRule(addressableGroup);
                newList.Add(addressRule);
            }

            _addressRules.Clear();
            foreach (var addressRule in newList)
                _addressRules.Add(addressRule);

            return true;
        }

        public bool TryProvideAddressAndAddressableGroup(
            string assetPath,
            Type assetType,
            bool isFolder,
            bool doSetup,
            out string address,
            out AddressableAssetGroup addressableGroup
        )
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

        /// <summary>
        ///     Provide the labels with addressable context.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <param name="address">The address assigned to the addressable entry.</param>
        /// <param name="addressableAssetGroup">The addressable asset group.</param>
        /// <param name="doSetup"></param>
        /// <param name="checkIsPathValidForEntry">
        ///     If true, check if the asset path is valid for entry.
        ///     You can pass false if it is guaranteed to be valid.
        /// </param>
        /// <returns></returns>
        public IReadOnlyCollection<string> ProvideLabels(
            string assetPath,
            Type assetType,
            bool isFolder,
            string address,
            AddressableAssetGroup addressableAssetGroup,
            bool doSetup,
            bool checkIsPathValidForEntry = true
        )
        {
            var labels = new HashSet<string>();
            for (int i = 0, count = _labelRules.Count; i < count; i++)
            {
                var labelRule = _labelRules[i];

                if (doSetup)
                    labelRule.Setup();

                if (labelRule.TryProvideLabel(assetPath, assetType, isFolder, address, addressableAssetGroup, out var label, checkIsPathValidForEntry))
                    labels.Add(label);
            }

            return labels;
        }

        public string ProvideVersion(string assetPath, Type assetType, bool isFolder, string address, AddressableAssetGroup addressableAssetGroup, bool doSetup)
        {
            foreach (var versionRule in _versionRules)
            {
                if (doSetup)
                    versionRule.Setup();

                // Adopt the first matching version.
                if (versionRule.TryProvideVersion(assetPath, assetType, isFolder, address, addressableAssetGroup, out var version))
                    return version;
            }

            return null;
        }

        public bool Validate(out LayoutRuleValidationError error)
        {
            var versionRuleErrors = new List<VersionRuleValidationError>();
            foreach (var versionRule in _versionRules)
                if (!versionRule.Validate(out var versionRuleError))
                    versionRuleErrors.Add(versionRuleError);

            var labelRuleErrors = new List<LabelRuleValidationError>();
            foreach (var labelRule in _labelRules)
                if (!labelRule.Validate(out var labelRuleError))
                    labelRuleErrors.Add(labelRuleError);

            var addressRuleErrors = new List<AddressRuleValidationError>();
            foreach (var addressRule in _addressRules)
                if (!addressRule.Validate(out var addressRuleError))
                    addressRuleErrors.Add(addressRuleError);

            if (versionRuleErrors.Count == 0 && labelRuleErrors.Count == 0 && addressRuleErrors.Count == 0)
            {
                error = null;
                return true;
            }

            error = new LayoutRuleValidationError(
                addressRuleErrors.ToArray(),
                labelRuleErrors.ToArray(),
                versionRuleErrors.ToArray());
            return false;
        }
    }
}
