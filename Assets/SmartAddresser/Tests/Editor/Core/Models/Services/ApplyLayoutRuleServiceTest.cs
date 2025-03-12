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
        private const string TestAnotherAssetName = "test_another_asset.asset";
        private const string TestAnotherAssetPath = "Assets/Tests/" + TestAnotherAssetName;

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
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            service.Apply(assetGuid, true, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Not.Null);
            Assert.That(assetEntry.GroupName, Is.EqualTo(TestAddressableGroupName));
            Assert.That(assetEntry.Address, Is.EqualTo(TestAssetName));
        }

        [Test]
        public void CreateEntryFromCompositeLayout()
        {
            var asset1Guid = GUID.Generate().ToString();
            var asset2Guid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule1 = CreateLayoutRule(TestAddressableGroupName, TestAssetPath, PartialAssetPathType.FileName);
            var layoutRule2 = CreateLayoutRule(TestAddressableGroupName, TestAnotherAssetPath,
                                               PartialAssetPathType.FileName);

            var assetDatabaseAdapter = new FakeAssetDatabaseAdapter();
            assetDatabaseAdapter.Entries.Add(new FakeAssetDatabaseAdapter.Entry(asset1Guid, TestAssetPath, assetType,
                                                                                    isFolder));
            assetDatabaseAdapter.Entries.Add(new FakeAssetDatabaseAdapter.Entry(asset2Guid, TestAnotherAssetPath,
                                                                                    assetType, isFolder));

            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(new[] { layoutRule1, layoutRule2 },
                                                     new UnityVersionExpressionParser(),
                                                     addressableSettingsAdapter,
                                                     assetDatabaseAdapter);

            layoutRule1.Setup();
            layoutRule2.Setup();

            service.Apply(asset1Guid, false, false);
            service.Apply(asset2Guid, false, false);

            var assetEntry1 = addressableSettingsAdapter.FindAssetEntry(asset1Guid);
            Assert.That(assetEntry1, Is.Not.Null);
            Assert.That(assetEntry1.GroupName, Is.EqualTo(TestAddressableGroupName));
            Assert.That(assetEntry1.Address, Is.EqualTo(TestAssetName));
            var assetEntry2 = addressableSettingsAdapter.FindAssetEntry(asset2Guid);
            Assert.That(assetEntry2, Is.Not.Null);
            Assert.That(assetEntry2.GroupName, Is.EqualTo(TestAddressableGroupName));
            Assert.That(assetEntry2.Address, Is.EqualTo(TestAnotherAssetName));
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
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            service.Apply(assetGuid, true, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Not.Null);
            Assert.That(assetEntry.GroupName, Is.EqualTo(TestAddressableGroupName));
            Assert.That(assetEntry.Address, Is.EqualTo(TestAssetName));
        }

        [Test]
        public void AddressIsNotAssignedAndBelongingToControlGroup_Removed()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName, TestAssetPath, PartialAssetPathType.FileName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            service.Apply(assetGuid, true, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Not.Null);

            // Clear the asset groups
            foreach (var addressRule in layoutRule.AddressRules)
                addressRule.AssetGroups.Clear();
            // Apply again
            service.Apply(assetGuid, true, false);
            // The entry should be removed.
            assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Null);
        }

        [Test]
        public void AddressIsNotAssignedAndBelongingToNotControlGroup_NotRemoved()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName, TestAssetPath, PartialAssetPathType.FileName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            service.Apply(assetGuid, true, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Not.Null);

            // Clear the asset groups
            foreach (var addressRule in layoutRule.AddressRules)
                addressRule.Control.Value = false;
            // Apply again
            service.Apply(assetGuid, true, false);
            // The entry should not be removed.
            assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Not.Null);
        }

        [Test]
        public void MatchedLayoutRuleNotExists_EntryIsNull()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName,
                "Assets/NotMatchedAssetPath.asset",
                PartialAssetPathType.FileName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            service.Apply(assetGuid, true, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Null);
        }

        [Test]
        public void GroupIsNull_EntryIsNull()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule =
                CreateLayoutRule((AddressableAssetGroup)null, TestAssetPath, PartialAssetPathType.FileName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            service.Apply(assetGuid, true, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Null);
        }

        [Test]
        public void VersionIsSatisfied()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName,
                TestAssetPath,
                PartialAssetPathType.FileName,
                version: "1.2.3",
                versionExpression: "[1.2.3,1.2.4)");
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            service.Apply(assetGuid, true, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Not.Null);
            Assert.That(assetEntry.GroupName, Is.EqualTo(TestAddressableGroupName));
            Assert.That(assetEntry.Address, Is.EqualTo(TestAssetName));
        }

        [Test]
        public void VersionIsNotSatisfied_EntryIsNull()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName,
                TestAssetPath,
                PartialAssetPathType.FileName,
                version: "1.2.3",
                versionExpression: "(1.2.3,1.3)");
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            service.Apply(assetGuid, true, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Null);
        }

        [Test]
        public void InvalidVersionExpression_Exception()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;

            var layoutRule = CreateLayoutRule(TestAddressableGroupName,
                TestAssetPath,
                PartialAssetPathType.FileName,
                version: "1.2.3",
                versionExpression:"(1.2.3, 1.3)");
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            Assert.That(() => service.Apply(assetGuid, true, false), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void SetLabel()
        {
            var assetGuid = GUID.Generate().ToString();
            var assetType = typeof(ScriptableObject);
            const bool isFolder = false;
            const string LabelName = "TestLabel";

            var layoutRule = CreateLayoutRule(TestAddressableGroupName,
                TestAssetPath,
                PartialAssetPathType.FileName,
                LabelName);
            var assetDatabaseAdapter =
                CreateSingleEntryAssetDatabaseAdapter(assetGuid, TestAssetPath, assetType, isFolder);
            var addressableSettingsAdapter = new FakeAddressableAssetSettingsAdapter();
            var service = new ApplyLayoutRuleService(layoutRule,
                new UnityVersionExpressionParser(),
                addressableSettingsAdapter,
                assetDatabaseAdapter);

            service.Apply(assetGuid, true, false);
            var assetEntry = addressableSettingsAdapter.FindAssetEntry(assetGuid);
            Assert.That(assetEntry, Is.Not.Null);
            Assert.That(assetEntry.Labels.Count, Is.EqualTo(1));
            Assert.That(assetEntry.Labels.First(), Is.EqualTo(LabelName));
        }

        private static FakeAssetDatabaseAdapter CreateSingleEntryAssetDatabaseAdapter(
            string guid,
            string assetPath,
            Type assetType,
            bool isValidFolder
        )
        {
            var assetDatabaseAdapter = new FakeAssetDatabaseAdapter();
            var entry = new FakeAssetDatabaseAdapter.Entry(guid, assetPath, assetType, isValidFolder);
            assetDatabaseAdapter.Entries.Add(entry);
            return assetDatabaseAdapter;
        }

        private static LayoutRule CreateLayoutRule(
            string addressableGroupName,
            string regexAssetPathFilter,
            PartialAssetPathType addressProvideMode,
            string label = null,
            string version = null,
            string versionExpression = null
        )
        {
            var addressableGroup = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            addressableGroup.Name = addressableGroupName;
            return CreateLayoutRule(addressableGroup, regexAssetPathFilter, addressProvideMode, label, version,
                                    versionExpression);
        }

        private static LayoutRule CreateLayoutRule(
            AddressableAssetGroup addressableGroup,
            string regexAssetPathFilter,
            PartialAssetPathType addressProvideMode,
            string label = null,
            string version = null,
            string versionExpression = null
        )
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

            layoutRule.Settings.VersionExpression.Value = versionExpression;

            return layoutRule;
        }
    }
}
