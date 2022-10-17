using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Layouts;
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
        /// <param name="layoutRule"></param>
        /// <returns></returns>
        public Layout Execute(LayoutRule layoutRule)
        {
            layoutRule.SetupForAddress();
            layoutRule.SetupForLabels();
            layoutRule.SetupForVersion();

            var assetPaths = _assetDatabaseAdapter.GetAllAssetPaths(); //TODO: 絞り込み
            var assetTypes = new Type[assetPaths.Length];
            var isFolders = new bool[assetPaths.Length];
            for (var i = 0; i < assetPaths.Length; i++)
            {
                assetTypes[i] = _assetDatabaseAdapter.GetMainAssetTypeAtPath(assetPaths[i]);
                isFolders[i] = _assetDatabaseAdapter.IsValidFolder(assetPaths[i]);
            }

            var layout = new Layout();
            foreach (var addressRule in layoutRule.AddressRules)
            {
                var group = new Group(addressRule.AddressableGroup);

                for (var i = 0; i < assetPaths.Length; i++)
                {
                    var assetPath = assetPaths[i];
                    var assetType = assetTypes[i];
                    var isFolder = isFolders[i];

                    // Address
                    addressRule.AddressProvider.Value.Provide(assetPath, assetType, isFolder);
                    if (!addressRule.TryProvideAddress(assetPath, assetType, isFolder, out var address))
                        continue;

                    // Labels
                    var labels = layoutRule.ProvideLabels(assetPath, assetType, isFolder, false).ToArray();

                    // Versions
                    var versions = new List<string>();
                    foreach (var versionRule in layoutRule.VersionRules)
                    {
                        if (!versionRule.TryProvideVersion(assetPath, assetType, isFolder, out var version))
                            continue;

                        versions.Add(version);
                    }

                    var entry = new Entry(address, assetPath, labels, versions.ToArray());
                    group.Entries.Add(entry);
                }

                if (group.Entries.Count >= 1)
                    layout.Groups.Add(group);
            }

            return layout;
        }
    }
}
