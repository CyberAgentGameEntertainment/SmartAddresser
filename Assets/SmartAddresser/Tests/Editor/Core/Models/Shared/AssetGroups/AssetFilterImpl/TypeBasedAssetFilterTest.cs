using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    internal sealed class TypeBasedAssetFilterTest
    {
        [Test]
        public void IsMatch_SetMatchedType_ReturnTrue()
        {
            var filter = new TypeBasedAssetFilter();
            filter.Type.Value = TypeReference.Create(typeof(Texture2D));
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test.png", typeof(Texture2D), false, null, null), Is.True);
        }

        [Test]
        public void IsMatch_SetDerivedType_ReturnTrue()
        {
            var filter = new TypeBasedAssetFilter();
            filter.Type.Value = TypeReference.Create(typeof(Texture));
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test.png", typeof(Texture2D), false, null, null), Is.True);
        }

        [Test]
        public void IsMatch_SetNotMatchedType_ReturnFalse()
        {
            var filter = new TypeBasedAssetFilter();
            filter.Type.Value = TypeReference.Create(typeof(Texture3D));
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test.png", typeof(Texture2D), false, null, null), Is.False);
        }

        [Test]
        public void IsMatch_ContainsMatched_ReturnTrue()
        {
            var filter = new TypeBasedAssetFilter();
            filter.Type.IsListMode = true;
            filter.Type.AddValue(TypeReference.Create(typeof(Texture3D)));
            filter.Type.AddValue(TypeReference.Create(typeof(Texture2D)));
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test.png", typeof(Texture2D), false, null, null), Is.True);
        }

        [Test]
        public void IsMatch_NotContainsMatched_ReturnTrue()
        {
            var filter = new TypeBasedAssetFilter();
            filter.Type.IsListMode = true;
            filter.Type.AddValue(TypeReference.Create(typeof(Texture3D)));
            filter.Type.AddValue(TypeReference.Create(typeof(Texture2D)));
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test.png", typeof(Texture2DArray), false, null, null), Is.False);
        }

        [Test]
        public void IsMatch_InvertMatchAndSetMatchedType_ReturnFalse()
        {
            var filter = new TypeBasedAssetFilter();
            filter.Type.Value = TypeReference.Create(typeof(Texture2D));
            filter.InvertMatch = true;
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test.png", typeof(Texture2D), false, null, null), Is.False);
        }

        [Test]
        public void IsMatch_InvertMatchAndSetNotMatchedType_ReturnTrue()
        {
            var filter = new TypeBasedAssetFilter();
            filter.Type.Value = TypeReference.Create(typeof(Texture3D));
            filter.InvertMatch = true;
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Assets/Test.png", typeof(Texture2D), false, null, null), Is.True);
        }

        [Test]
        public void Validate_Valid_ReturnTrue()
        {
            var filter = new TypeBasedAssetFilter();
            var invalidTypeReference = TypeReference.Create(typeof(Texture2D));
            filter.Type.Value = invalidTypeReference;
            filter.SetupForMatching();

            Assert.That(filter.Validate(out _), Is.True);
        }

        [Test]
        public void Validate_Invalid_ReturnFalse()
        {
            var filter = new TypeBasedAssetFilter();
            var invalidTypeReference = new TypeReference();
            invalidTypeReference.Name = "Name";
            invalidTypeReference.FullName = "FullName";
            invalidTypeReference.AssemblyQualifiedName = "AssemblyQualifiedName";
            filter.Type.Value = invalidTypeReference;
            filter.SetupForMatching();

            Assert.That(filter.Validate(out _), Is.False);
        }
    }
}
