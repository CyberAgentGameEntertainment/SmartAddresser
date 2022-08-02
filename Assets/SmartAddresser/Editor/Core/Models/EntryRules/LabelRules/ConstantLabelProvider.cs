using System;
using SmartAddresser.Editor.Core.Models.Shared;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.EntryRules.LabelRules
{
    /// <summary>
    ///     Provide constant label.
    /// </summary>
    [Serializable]
    public sealed class ConstantLabelProvider : ILabelProvider
    {
        [SerializeField] private string _label;

        public string Label
        {
            get => _label;
            set => _label = value;
        }

        void IProvider<string>.Setup()
        {
        }

        string IProvider<string>.Provide(string assetPath, Type assetType, bool isFolder)
        {
            return _label;
        }

        public string GetDescription()
        {
            return $"Constant: {_label}";
        }
    }
}
