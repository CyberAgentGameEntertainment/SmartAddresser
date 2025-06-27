using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Models.Layouts;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;

namespace SmartAddresser.Editor.Core.Models.Services
{
    /// <summary>
    ///     Service to build the <see cref="Layout" /> from the <see cref="LayoutRule" />.
    /// </summary>
    public sealed class BuildLayoutService
    {
        private readonly IAssetDatabaseAdapter _assetDatabaseAdapter;

        public BuildLayoutService(IAssetDatabaseAdapter assetDatabaseAdapter)
        {
            _assetDatabaseAdapter = assetDatabaseAdapter;
        }

        /// <summary>
        ///     Build the <see cref="Layout" /> from the <see cref="LayoutRule" />.
        /// </summary>
        /// <param name="doSetup"></param>
        /// <param name="layoutRule"></param>
        /// <returns></returns>
        public Layout Execute(bool doSetup, LayoutRule layoutRule)
        {
            if (doSetup)
                layoutRule.Setup();

            var assetPaths = _assetDatabaseAdapter
                .GetAllAssetPaths()
                .Where(AddressableAssetUtility.IsAssetPathValidForEntry)
                .ToArray();

            var assetTypes = new Type[assetPaths.Length];
            var isFolders = new bool[assetPaths.Length];
            for (var i = 0; i < assetPaths.Length; i++)
            {
                assetTypes[i] = _assetDatabaseAdapter.GetMainAssetTypeAtPath(assetPaths[i]);
                isFolders[i] = _assetDatabaseAdapter.IsValidFolder(assetPaths[i]);
            }

            var layout = new Layout();

            var buildGroupsTasks = layoutRule.AddressRules
                .Where(addressRule => addressRule.Control.Value)
                .Select(addressRule => BuildGroupAsync(addressRule, layoutRule, assetPaths, assetTypes, isFolders));

            var groups = Task.WhenAll(buildGroupsTasks).Result;

            foreach (var group in groups)
                if (group != null && group.Entries.Count >= 1)
                    layout.Groups.Add(group);

            return layout;
        }

        private static Task<Group> BuildGroupAsync(
            AddressRule addressRule,
            LayoutRule layoutRule,
            IReadOnlyList<string> assetPaths,
            IReadOnlyList<Type> assetTypes,
            IReadOnlyList<bool> isFolders
        )
        {
            if (!addressRule.Control.Value)
                return Task.FromResult((Group)null);

            return Task.Run(() =>
            {
                var group = new Group(addressRule.AddressableGroup);

                for (var i = 0; i < assetPaths.Count; i++)
                {
                    var assetPath = assetPaths[i];
                    var assetType = assetTypes[i];
                    var isFolder = isFolders[i];

                    // Address
                    if (!addressRule.TryProvideAddress(assetPath, assetType, isFolder, out var address, false))
                        continue;

                    // Labels
                    var labels = layoutRule.ProvideLabels(assetPath, assetType, isFolder, address, addressRule.AddressableGroup, false, false).ToArray();

                    // Versions
                    var versions = new List<string>();
                    foreach (var versionRule in layoutRule.VersionRules)
                    {
                        if (!versionRule.TryProvideVersion(assetPath, assetType, isFolder, address, addressRule.AddressableGroup, out var version, false))
                            continue;

                        versions.Add(version);
                    }

                    var entry = new Entry(address, assetPath, labels, versions);
                    group.Entries.Add(entry);
                }

                return group;
            });
        }
    }
}
