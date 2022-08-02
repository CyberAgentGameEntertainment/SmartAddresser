using System;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.TagRules
{
    /// <summary>
    ///     Provide constant tag.
    /// </summary>
    [Serializable]
    public sealed class ConstantTagProvider : ITagProvider
    {
        [SerializeField] private string _tag;

        public string Tag
        {
            get => _tag;
            set => _tag = value;
        }

        void IProvider<string>.Setup()
        {
        }

        string IProvider<string>.Provide(string assetPath, Type assetType, bool isFolder)
        {
            return _tag;
        }

        public string GetDescription()
        {
            return $"Constant: {_tag}";
        }
    }
}
