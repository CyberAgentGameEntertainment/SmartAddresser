using System;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Tests.Editor.Core.Models.Shared;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SmartAddresser.Tests.Editor.Core.Models.Services
{
    internal sealed class BuildLayoutServiceTest
    {
        [Test]
        public void LayoutRuleContainsTargetAsset_LayoutIsCreated()
        {
            const string assetPath = "Assets/TestObj.asset";
            const string addressableGroupName = "Test Addressable Group";
            var labels = new[] { "Test Label 01", "Test Label 02" };
            var versions = new[] { "1.0.0", "2.0.0" };

            var adapter = CreateSingleEntryAssetDatabaseAdapter("", assetPath, typeof(Object), false);
            var service = new BuildLayoutService(adapter);
            var layoutRule = CreateLayoutRule(addressableGroupName, assetPath, PartialAssetPathType.AssetPath, labels,
                versions);
            var layout = service.Execute(true, layoutRule);

            Assert.That(layout.Groups.Count, Is.EqualTo(1));
            Assert.That(layout.Groups[0].AddressableGroup.Name, Is.EqualTo(addressableGroupName));
            Assert.That(layout.Groups[0].Entries.Count, Is.EqualTo(1));
            Assert.That(layout.Groups[0].Entries[0].Address, Is.EqualTo(assetPath));
            Assert.That(layout.Groups[0].Entries[0].Labels[0], Is.EqualTo(labels[0]));
            Assert.That(layout.Groups[0].Entries[0].Labels[1], Is.EqualTo(labels[1]));
            Assert.That(layout.Groups[0].Entries[0].Versions[0], Is.EqualTo(versions[0]));
            Assert.That(layout.Groups[0].Entries[0].Versions[1], Is.EqualTo(versions[1]));
        }

        [Test]
        public void LayoutRuleNotContainsTargetAsset_GroupIsNotCreated()
        {
            const string ruleAssetPath = "Assets/RuleTestObj.asset";
            const string targetAssetPath = "Assets/TargetTestObj.asset";
            const string addressableGroupName = "Test Addressable Group";

            var adapter = CreateSingleEntryAssetDatabaseAdapter("", targetAssetPath, typeof(Object), false);
            var service = new BuildLayoutService(adapter);
            var layoutRule = CreateLayoutRule(addressableGroupName, ruleAssetPath, PartialAssetPathType.AssetPath);
            var layout = service.Execute(true, layoutRule);

            Assert.That(layout.Groups.Count, Is.EqualTo(0));
        }

        [Test]
        public void TargetAssetIsScript_GroupIsNotCreated()
        {
            const string assetPath = "Assets/RuleTestObj.cs";
            const string addressableGroupName = "Test Addressable Group";

            var adapter = CreateSingleEntryAssetDatabaseAdapter("", assetPath, typeof(Object), false);
            var service = new BuildLayoutService(adapter);
            var layoutRule = CreateLayoutRule(addressableGroupName, assetPath, PartialAssetPathType.AssetPath);
            var layout = service.Execute(true, layoutRule);

            Assert.That(layout.Groups.Count, Is.EqualTo(0));
        }

        private static FakeAssetDatabaseAdapter CreateSingleEntryAssetDatabaseAdapter(string guid, string assetPath,
            Type assetType, bool isValidFolder)
        {
            var assetDatabaseAdapter = new FakeAssetDatabaseAdapter();
            var entry = new FakeAssetDatabaseAdapter.Entry(guid, assetPath, assetType, isValidFolder);
            assetDatabaseAdapter.Entries.Add(entry);
            return assetDatabaseAdapter;
        }

        private static LayoutRule CreateLayoutRule(string addressableGroupName, string regexAssetPathFilter,
            PartialAssetPathType addressProvideMode, string[] labels = null, string[] versions = null)
        {
            var addressableGroup = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            addressableGroup.Name = addressableGroupName;
            return CreateLayoutRule(addressableGroup, regexAssetPathFilter, addressProvideMode, labels, versions);
        }

        private static LayoutRule CreateLayoutRule(AddressableAssetGroup addressableGroup, string regexAssetPathFilter,
            PartialAssetPathType addressProvideMode, string[] labels = null, string[] versions = null)
        {
            var assetFilter = new RegexBasedAssetFilter
            {
                AssetPathRegex =
                {
                    Value = regexAssetPathFilter
                }
            };

            var assetGroup = new AssetGroup();
            assetGroup.Filters.Add(assetFilter);

            var addressProvider = new AssetPathBasedAddressProvider
            {
                Source = addressProvideMode,
                ReplaceWithRegex = false
            };

            var addressRule = new AddressRule(addressableGroup);
            addressRule.Control.Value = true;
            addressRule.AddressProvider.Value = addressProvider;
            addressRule.AssetGroups.Add(assetGroup);

            var layoutRule = new LayoutRule();
            layoutRule.AddressRules.Add(addressRule);

            if (labels != null)
                foreach (var label in labels)
                {
                    var labelRule = new LabelRule();
                    var labelProvider = new ConstantLabelProvider
                    {
                        Label = label
                    };
                    labelRule.LabelProvider.Value = labelProvider;
                    labelRule.AssetGroups.Add(assetGroup);
                    layoutRule.LabelRules.Add(labelRule);
                }

            if (versions != null)
                foreach (var version in versions)
                {
                    var versionRule = new VersionRule();
                    var versionProvider = new ConstantVersionProvider
                    {
                        Version = version
                    };
                    versionRule.VersionProvider.Value = versionProvider;
                    versionRule.AssetGroups.Add(assetGroup);
                    layoutRule.VersionRules.Add(versionRule);
                }

            return layoutRule;
        }
    }
}
