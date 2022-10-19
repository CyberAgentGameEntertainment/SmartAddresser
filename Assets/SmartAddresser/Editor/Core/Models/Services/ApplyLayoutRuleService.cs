using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Foundation.AddressableAdapter;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using SmartAddresser.Editor.Foundation.SemanticVersioning;

namespace SmartAddresser.Editor.Core.Models.Services
{
    public sealed class ApplyLayoutRuleService
    {
        private readonly IAddressableAssetSettingsAdapter _addressableSettingsAdapter;
        private readonly IAssetDatabaseAdapter _assetDatabaseAdapter;
        private readonly LayoutRule _layoutRule;
        private readonly IVersionExpressionParser _versionExpressionParser;

        public ApplyLayoutRuleService(LayoutRule layoutRule,
            IVersionExpressionParser versionExpressionParser,
            IAddressableAssetSettingsAdapter addressableSettingsAdapter,
            IAssetDatabaseAdapter assetDatabaseAdapter)
        {
            _layoutRule = layoutRule;
            _addressableSettingsAdapter = addressableSettingsAdapter;
            _versionExpressionParser = versionExpressionParser;
            _assetDatabaseAdapter = assetDatabaseAdapter;
        }

        /// <summary>
        ///     If you want to process multiple asset by this instance, you should call this method before you call
        ///     <see cref="Execute" /> and set <c>false</c> to the <c>doSetup</c> argument of the <see cref="Execute" />.
        ///     If you want to process single asset by this instance, you should not call this method and set <c>true</c> to the
        ///     <c>doSetup</c> argument of the <see cref="Execute" />.
        /// </summary>
        public void Setup()
        {
            _layoutRule.SetupForAddress();
            _layoutRule.SetupForLabels();
            _layoutRule.SetupForVersion();
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
        /// <param name="versionExpression"></param>
        /// <returns>
        ///     If the layout rule was applied to the addressable asset system, return true.
        ///     Returns false if no suitable layout rule was found.
        /// </returns>
        public bool Execute(string assetGuid, bool doSetup, string versionExpression = null)
        {
            var assetPath = _assetDatabaseAdapter.GUIDToAssetPath(assetGuid);
            var assetType = _assetDatabaseAdapter.GetMainAssetTypeAtPath(assetPath);
            var isFolder = _assetDatabaseAdapter.IsValidFolder(assetPath);

            // If the layout rule was not found, return false.
            if (!_layoutRule.TryProvideAddressAndAddressableGroup(assetPath, assetType, isFolder, doSetup, out var address,
                    out var addressableGroup))
                return false;

            // If the layout rule is found but the addressable asset group has already been destroyed, return false.
            if (addressableGroup == null)
                return false;

            var addressableGroupName = addressableGroup.Name;

            // Check the version if it is specified.
            if (!string.IsNullOrEmpty(versionExpression))
            {
                var comparator = _versionExpressionParser.CreateComparator(versionExpression);
                var versionText = _layoutRule.ProvideVersion(assetPath, assetType, isFolder, doSetup);

                // If the version is not satisfied, return false.
                if (!string.IsNullOrEmpty(versionText)
                    && Version.TryCreate(versionText, out var version)
                    && !comparator.IsSatisfied(version))
                    return false;
            }

            // Set group and address.
            var entryAdapter = _addressableSettingsAdapter.CreateOrMoveEntry(addressableGroupName, assetGuid);
            entryAdapter.SetAddress(address);

            // Add labels to addressable settings if not exists.
            var labels = _layoutRule.ProvideLabels(assetPath, assetType, isFolder, doSetup);
            var addressableLabels = _addressableSettingsAdapter.GetLabels();
            foreach (var label in labels)
                if (!addressableLabels.Contains(label))
                    _addressableSettingsAdapter.AddLabel(label);

            // Set labels.
            foreach (var label in entryAdapter.Labels)
                entryAdapter.SetLabel(label, false);
            foreach (var label in labels)
                entryAdapter.SetLabel(label, true);

            return true;
        }
    }
}
