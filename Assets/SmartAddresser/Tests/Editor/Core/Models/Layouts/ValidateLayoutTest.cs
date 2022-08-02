using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Layouts;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.Layouts
{
    internal sealed class ValidateLayoutTest
    {
        [Test]
        public void ValidLayout_ErrorTypeIsNone()
        {
            var layout = new Layout();

            var addressableGroup1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group1 = new Group(addressableGroup1);
            var entry1 = new Entry("dummyAddress1", "Assets/dummy_asset1.asset", null, null);
            group1.Entries.Add(entry1);
            layout.Groups.Add(group1);

            var addressableGroup2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = new Group(addressableGroup2);
            var entry2 = new Entry("dummyAddress2", "Assets/dummy_asset2.asset", null, null);
            group2.Entries.Add(entry2);
            layout.Groups.Add(group2);

            layout.Validate();

            Assert.That(layout.ErrorType, Is.EqualTo(LayoutErrorType.None));
            Assert.That(layout.Groups[0].ErrorType, Is.EqualTo(LayoutErrorType.None));
            Assert.That(layout.Groups[0].Entries[0].ErrorType, Is.EqualTo(LayoutErrorType.None));
            Assert.That(layout.Groups[1].ErrorType, Is.EqualTo(LayoutErrorType.None));
            Assert.That(layout.Groups[1].Entries[0].ErrorType, Is.EqualTo(LayoutErrorType.None));
        }

        [Test]
        public void SameAssetIsIncludedInTwoGroups_ErrorTypeIsError()
        {
            var layout = new Layout();
            const string dummyAssetPath = "Assets/dummy_asset.asset";

            var addressableGroup1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group1 = new Group(addressableGroup1);
            var entry1 = new Entry("dummyAddress1", dummyAssetPath, null, null);
            group1.Entries.Add(entry1);
            layout.Groups.Add(group1);

            var addressableGroup2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = new Group(addressableGroup2);
            var entry2 = new Entry("dummyAddress2", dummyAssetPath, null, null);
            group2.Entries.Add(entry2);
            layout.Groups.Add(group2);

            layout.Validate();

            Assert.That(layout.ErrorType, Is.EqualTo(LayoutErrorType.Error));
            Assert.That(layout.Groups[0].ErrorType, Is.EqualTo(LayoutErrorType.Error));
            Assert.That(layout.Groups[0].Entries[0].ErrorType, Is.EqualTo(LayoutErrorType.Error));
            Assert.That(layout.Groups[1].ErrorType, Is.EqualTo(LayoutErrorType.Error));
            Assert.That(layout.Groups[1].Entries[0].ErrorType, Is.EqualTo(LayoutErrorType.Error));
        }

        [Test]
        public void SameAddressIsIncludedInTwoGroups_ErrorTypeIsWarning()
        {
            var layout = new Layout();
            const string dummyAddress = "dummyAddress";

            var addressableGroup1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group1 = new Group(addressableGroup1);
            var entry1 = new Entry(dummyAddress, "Assets/dummy_asset1.asset", null, null);
            group1.Entries.Add(entry1);
            layout.Groups.Add(group1);

            var addressableGroup2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = new Group(addressableGroup2);
            var entry2 = new Entry(dummyAddress, "Assets/dummy_asset2.asset", null, null);
            group2.Entries.Add(entry2);
            layout.Groups.Add(group2);

            layout.Validate();

            Assert.That(layout.ErrorType, Is.EqualTo(LayoutErrorType.Warning));
            Assert.That(layout.Groups[0].ErrorType, Is.EqualTo(LayoutErrorType.Warning));
            Assert.That(layout.Groups[0].Entries[0].ErrorType, Is.EqualTo(LayoutErrorType.Warning));
            Assert.That(layout.Groups[1].ErrorType, Is.EqualTo(LayoutErrorType.Warning));
            Assert.That(layout.Groups[1].Entries[0].ErrorType, Is.EqualTo(LayoutErrorType.Warning));
        }

        [Test]
        public void SameAssetPathAndSameAddressIsIncludedInTwoGroups_ErrorTypeIsError()
        {
            var layout = new Layout();
            const string dummyAssetPath = "Assets/dummy_asset.asset";
            const string dummyAddress = "dummyAddress";

            var addressableGroup1 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group1 = new Group(addressableGroup1);
            var entry1 = new Entry(dummyAddress, dummyAssetPath, null, null);
            group1.Entries.Add(entry1);
            layout.Groups.Add(group1);

            var addressableGroup2 = ScriptableObject.CreateInstance<AddressableAssetGroup>();
            var group2 = new Group(addressableGroup2);
            var entry2 = new Entry(dummyAddress, dummyAssetPath, null, null);
            group2.Entries.Add(entry2);
            layout.Groups.Add(group2);

            layout.Validate();

            Assert.That(layout.ErrorType, Is.EqualTo(LayoutErrorType.Error));
            Assert.That(layout.Groups[0].ErrorType, Is.EqualTo(LayoutErrorType.Error));
            Assert.That(layout.Groups[0].Entries[0].ErrorType, Is.EqualTo(LayoutErrorType.Error));
            Assert.That(layout.Groups[1].ErrorType, Is.EqualTo(LayoutErrorType.Error));
            Assert.That(layout.Groups[1].Entries[0].ErrorType, Is.EqualTo(LayoutErrorType.Error));
        }
    }
}
