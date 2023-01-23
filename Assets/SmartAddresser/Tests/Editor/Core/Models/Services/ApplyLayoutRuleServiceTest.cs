using System;
using System.Linq;
using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules;
using SmartAddresser.Editor.Core.Models.LayoutRules.VersionRules;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.SemanticVersioning;
using SmartAddresser.Tests.Editor.Core.Models.Shared;
using SmartAddresser.Tests.Editor.Foundation;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.Services
{
    internal sealed class ApplyLayoutRuleServiceTest
    {
        private const string TestAddressableGroupName = "TestGroup";
        private const string TestAssetName = "test_asset.asset";
        private const string TestAssetPath = "Assets/Tests/" + TestAssetName;

        [Test]
        public void CreateEntry()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName, TestAssetPath, PartialAssetPathType.FileName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule, new UnityVersionExpressionParser(),
                addressableSettingsAdapter, assetDatabaseAdapter);

            var result = service.TryAddEntry(assetGuid, true);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(result, Is.True);
            Assert.That(assetEntry, Is.Not.Null);
            Assert.That(assetEntry.GroupName, Is.EqualTo(TestAddressableGroupName));
            Assert.That(assetEntry.Address, Is.EqualTo(TestAssetName));
        }

        [Test]
        public void PreSetup()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName, TestAssetPath, PartialAssetPathType.FileName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule, new UnityVersionExpressionParser(),
                addressableSettingsAdapter, assetDatabaseAdapter);

            service.Setup();
            var result = service.TryAddEntry(assetGuid, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(result, Is.True);
            Assert.That(assetEntry, Is.Not.Null);
            Assert.That(assetEntry.GroupName, Is.EqualTo(TestAddressableGroupName));
            Assert.That(assetEntry.Address, Is.EqualTo(TestAssetName));
        }

        [Test]
        public void MatchedLayoutRuleNotExists_ReturnFalse()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName, "Assets/NotMatchedAssetPath.asset",
                PartialAssetPathType.FileName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule, new UnityVersionExpressionParser(),
                addressableSettingsAdapter, assetDatabaseAdapter);

            var result = service.TryAddEntry(assetGuid, true);
            Assert.That(result, Is.False);
        }

        [Test]
        public void GroupIsNull_ReturnFalse()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule =
                CreateLayoutRule((AddressableAssetGroup)null, TestAssetPath, PartialAssetPathType.FileName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule, new UnityVersionExpressionParser(),
                addressableSettingsAdapter, assetDatabaseAdapter);

            var result = service.TryAddEntry(assetGuid, true);
            Assert.That(result, Is.False);
        }

        [Test]
        public void VersionIsSatisfied()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName, TestAssetPath, PartialAssetPathType.FileName,
                version: "1.2.3");
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule, new UnityVersionExpressionParser(),
                addressableSettingsAdapter, assetDatabaseAdapter);

            var result = service.TryAddEntry(assetGuid, true, "[1.2.3,1.2.4)");
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(result, Is.True);
            Assert.That(assetEntry, Is.Not.Null);
            Assert.That(assetEntry.GroupName, Is.EqualTo(TestAddressableGroupName));
            Assert.That(assetEntry.Address, Is.EqualTo(TestAssetName));
        }

        [Test]
        public void VersionIsNotSatisfied_ReturnFalse()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName, TestAssetPath, PartialAssetPathType.FileName,
                version: "1.2.3");
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule, new UnityVersionExpressionParser(),
                addressableSettingsAdapter, assetDatabaseAdapter);

            var result = service.TryAddEntry(assetGuid, true, "(1.2.3,1.3)");
            Assert.That(result, Is.False);
        }

        [Test]
        public void InvalidVersionExpression_Exception()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName, TestAssetPath, PartialAssetPathType.FileName,
                version: "1.2.3");
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule, new UnityVersionExpressionParser(),
                addressableSettingsAdapter, assetDatabaseAdapter);

            Assert.That(() => service.TryAddEntry(assetGuid, true, "(1.2.3, 1.3)"), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void SetLabel()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;
            const string LabelName = "TestLabel";

            var layoutRule = CreateLayoutRule(TestAddressableGroupName, TestAssetPath, PartialAssetPathType.FileName,
                LabelName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule, new UnityVersionExpressionParser(),
                addressableSettingsAdapter, assetDatabaseAdapter);

            var result = service.TryAddEntry(assetGuid, true);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(result, Is.True);
            Assert.That(assetEntry, Is.Not.Null);
            Assert.That(assetEntry.Labels.Count, Is.EqualTo(1));
            Assert.That(assetEntry.Labels.First(), Is.EqualTo(LabelName));
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
            PartialAssetPathType addressProvideMode, string label = null, string version = null)
        {
            var addressableGroup = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            addressableGroup.Name = addressableGroupName;
            return CreateLayoutRule(addressableGroup, regexAssetPathFilter, addressProvideMode, label, version);
        }

        private static LayoutRule CreateLayoutRule(AddressableAssetGroup addressableGroup, string regexAssetPathFilter,
            PartialAssetPathType addressProvideMode, string label = null, string version = null)
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

            if (!string.IsNullOrEmpty(label))
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

            if (!string.IsNullOrEmpty(version))
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
