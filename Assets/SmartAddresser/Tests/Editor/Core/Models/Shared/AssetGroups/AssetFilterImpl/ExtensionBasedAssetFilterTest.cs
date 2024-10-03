// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using NUnit.Framework;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using UnityEngine;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl
{
    internal sealed class ExtensionBasedAssetFilterTest
    {
        [Test]
        public void IsMatch_RegisterMatchedExtension_ReturnTrue()
        {
            var filter = new ExtensionBasedAssetFilter();
            filter.Extension.Value = "png";
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Test.png", typeof(Texture2D), false), Is.True);
        }

        [Test]
        public void IsMatch_RegisterMatchedExtensionWithDot_ReturnTrue()
        {
            var filter = new ExtensionBasedAssetFilter();
            filter.Extension.Value = ".png";
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Test.png", typeof(Texture2D), false), Is.True);
        }

        [Test]
        public void IsMatch_RegisterNotMatchedExtension_ReturnFalse()
        {
            var filter = new ExtensionBasedAssetFilter();
            filter.Extension.Value = "jpg";
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Test.png", typeof(Texture2D), false), Is.False);
        }

        [Test]
        public void IsMatch_RegisterExtensionsAndContainsMatched_ReturnTrue()
        {
            var filter = new ExtensionBasedAssetFilter();
            filter.Extension.IsListMode = true;
            filter.Extension.AddValue("png");
            filter.Extension.AddValue("jpg");
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Test.png", typeof(Texture2D), false), Is.True);
        }

        [Test]
        public void IsMatch_RegisterExtensionsAndNotContainsMatched_ReturnFalse()
        {
            var filter = new ExtensionBasedAssetFilter();
            filter.Extension.IsListMode = true;
            filter.Extension.AddValue("jpg");
            filter.Extension.AddValue("exr");
            filter.SetupForMatching();
            Assert.That(filter.IsMatch("Test.png", typeof(Texture2D), false), Is.False);
        }

        [Test]
        public void Validate_ValidExtension_ReturnTrue()
        {
            var filter = new ExtensionBasedAssetFilter();
            filter.Extension.Value = "png";
            
            Assert.That(filter.Validate(out _), Is.True);
        }
        
        [Test]
        public void Validate_ExtensionIsNull_ReturnFalse()
        {
            var filter = new ExtensionBasedAssetFilter();
            filter.SetupForMatching();
            
            Assert.That(filter.Validate(out _), Is.False);
        }
    }
}
