using System;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.LabelRules
{
    [Serializable]
    public sealed class CustomLabelProvider : ILabelProvider
    {
        public LabelProviderAsset labelProvider;

        public void Setup()
        {
            labelProvider.Setup();
        }

        public string Provide(string assetPath, Type assetType, bool isFolder)
        {
            return labelProvider.Provide(assetPath, assetType, isFolder);
        }

        public string GetDescription()
        {
            if (labelProvider == null)
                return string.Empty;

            return labelProvider.GetDescription();
        }
    }
}
