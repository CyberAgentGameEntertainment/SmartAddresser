using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Foundation.AddressableAdapter;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using SmartAddresser.Editor.Foundation.SemanticVersioning;
using UnityEditor.AddressableAssets.Settings;
using Version = SmartAddresser.Editor.Foundation.SemanticVersioning.Version;

namespace SmartAddresser.Editor.Core.Models.Services
{
    public sealed class ApplyLayoutRuleService
    {
        private readonly IAddressableAssetSettingsAdapter _addressableSettingsAdapter;
        private readonly IAssetDatabaseAdapter _assetDatabaseAdapter;
        private readonly LayoutRule[] _layoutRules;
        private readonly IVersionExpressionParser _versionExpressionParser;

        public ApplyLayoutRuleService(
            LayoutRule layoutRule,
            IVersionExpressionParser versionExpressionParser,
            IAddressableAssetSettingsAdapter addressableSettingsAdapter,
            IAssetDatabaseAdapter assetDatabaseAdapter
        ) : this(new[] { layoutRule }, versionExpressionParser, addressableSettingsAdapter, assetDatabaseAdapter)
        {
        }

        public ApplyLayoutRuleService(
            IEnumerable<LayoutRule> layoutRules,
            IVersionExpressionParser versionExpressionParser,
            IAddressableAssetSettingsAdapter addressableSettingsAdapter,
            IAssetDatabaseAdapter assetDatabaseAdapter
        )
        {
            _layoutRules = layoutRules.ToArray();
            _addressableSettingsAdapter = addressableSettingsAdapter;
            _versionExpressionParser = versionExpressionParser;
            _assetDatabaseAdapter = assetDatabaseAdapter;
        }

        /// <summary>
        ///     Apply the layout rule to the addressable settings for all assets.
        /// </summary>
        public void ApplyAll(bool doSetup)
        {
            foreach (var layoutRule in _layoutRules)
                if (doSetup)
                    layoutRule.Setup();

            // Add all entries to the addressable asset system.
            var removeTargetAssetGuids = new List<string>();
            foreach (var assetPath in _assetDatabaseAdapter.GetAllAssetPaths())
            {
                var guid = _assetDatabaseAdapter.AssetPathToGUID(assetPath);
                var result =
                    _layoutRules.Any(x => TryAddEntry(x, guid, false, false, x.Settings.VersionExpression.Value));

                if (!result)
                    removeTargetAssetGuids.Add(guid);
            }

            // If the address is not assigned by the LayoutRule and the entry belongs to the AddressableGroup under Control, remove the entry.
            var controlGroupNames = _layoutRules
                                    .SelectMany(x => x.AddressRules)
                                    .Where(x => x.Control.Value)
                                    .Select(x => x.AddressableGroup.Name)
                                    .ToArray();
            foreach (var guid in removeTargetAssetGuids)
            {
                var entryAdapter = _addressableSettingsAdapter.FindAssetEntry(guid);
                if (entryAdapter == null)
                    continue;

                if (controlGroupNames.Contains(entryAdapter.GroupName))
                    _addressableSettingsAdapter.RemoveEntry(guid, false);
            }

            _addressableSettingsAdapter.InvokeBatchModificationEvent();
        }

        public void Apply(string assetGuid, bool doSetup, bool invokeModificationEvent)
        {
            var result = _layoutRules.Any(x => TryAddEntry(x, assetGuid, doSetup, false, x.Settings.VersionExpression.Value));

            // If the address is not assigned by the LayoutRule and the entry belongs to the AddressableGroup under Control, remove the entry.
            var controlGroupNames = _layoutRules
                                    .SelectMany(x => x.AddressRules)
                                    .Where(x => x.Control.Value)
                                    .Select(x => x.AddressableGroup.Name);
            if (!result)
            {
                var entryAdapter = _addressableSettingsAdapter.FindAssetEntry(assetGuid);
                if (entryAdapter != null && controlGroupNames.Contains(entryAdapter.GroupName))
                    _addressableSettingsAdapter.RemoveEntry(assetGuid, false);
            }

            if (invokeModificationEvent)
                _addressableSettingsAdapter.InvokeBatchModificationEvent();
        }

        /// <summary>
        ///     Apply the layout rule to the addressable settings.
        /// </summary>
        /// <param name="assetGuid"></param>
        /// <param name="doSetup">
        ///     If true, setup rules before providing.
        ///     When you call this method multiple times, set this false and call <see cref="Setup" /> before.
        ///     If you call this method only once, set this true and don't call <see cref="Setup" />.
        /// </param>
        /// <param name="invokeModificationEvent">
        ///     If true, call <see cref="AddressableAssetSettings.OnModification" /> after
        ///     creating or moving.
        /// </param>
        /// <param name="versionExpression"></param>
        /// <returns>
        ///     If the layout rule was applied to the addressable asset system, return true.
        ///     Returns false if no suitable layout rule was found.
        /// </returns>
        private bool TryAddEntry(
            LayoutRule layoutRule,
            string assetGuid,
            bool doSetup,
            bool invokeModificationEvent,
            string versionExpression = null
        )
        {
            var assetPath = _assetDatabaseAdapter.GUIDToAssetPath(assetGuid);
            var assetType = _assetDatabaseAdapter.GetMainAssetTypeAtPath(assetPath);
            var isFolder = _assetDatabaseAdapter.IsValidFolder(assetPath);

            // If the layout rule was not found, return false.
            if (!layoutRule.TryProvideAddressAndAddressableGroup(assetPath,
                                                                 assetType,
                                                                 isFolder,
                                                                 doSetup,
                                                                 out var address,
                                                                 out var addressableGroup))
                return false;

            // If the layout rule is found but the addressable asset group has already been destroyed, return false.
            if (addressableGroup == null)
                return false;

            // Check the version if it is specified.
            if (!string.IsNullOrEmpty(versionExpression))
            {
                var comparator = _versionExpressionParser.CreateComparator(versionExpression);
                var versionText = layoutRule.ProvideVersion(assetPath, assetType, isFolder, address, addressableGroup, doSetup);

                if (string.IsNullOrEmpty(versionText) && layoutRule.Settings.ExcludeUnversioned.Value)
                    return false;

                // If the version is not satisfied, return false.
                if (!string.IsNullOrEmpty(versionText)
                    && Version.TryCreate(versionText, out var version)
                    && !comparator.IsSatisfied(version))
                    return false;
            }

            // Set group and address.
            var entryAdapter =
                _addressableSettingsAdapter.CreateOrMoveEntry(addressableGroup.Name, assetGuid, invokeModificationEvent);
            entryAdapter.SetAddress(address, invokeModificationEvent);

            // Provide labels with addressable context (address and group).
            var labels = layoutRule.ProvideLabels(assetPath, assetType, isFolder, address, addressableGroup, doSetup);
            
            // Add labels to addressable settings if not exists.
            var addressableLabels = _addressableSettingsAdapter.GetLabels();
            foreach (var label in labels)
                if (!addressableLabels.Contains(label))
                    _addressableSettingsAdapter.AddLabel(label, invokeModificationEvent);

            // Remove old labels.
            var oldLabels = entryAdapter.Labels.ToArray();
            foreach (var label in oldLabels)
                entryAdapter.SetLabel(label, false, invokeModificationEvent);

            // Add new labels.
            foreach (var label in labels)
                entryAdapter.SetLabel(label, true, invokeModificationEvent);

            return true;
        }

        public void InvokeBatchModificationEvent()
        {
            _addressableSettingsAdapter.InvokeBatchModificationEvent();
        }
    }
}
