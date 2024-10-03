// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Text;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups
{
    /// <summary>
    ///     Rules for defining a group of assets.
    /// </summary>
    [Serializable]
    public sealed class AssetGroup
    {
        private const string Indent = "    ";

        [SerializeField] private string _id;
        [SerializeField] private ObservableProperty<string> _name = new ObservableProperty<string>("New Asset Group");

        [SerializeField] private SerializeReferenceObservableList<IAssetFilter> _filters =
            new SerializeReferenceObservableList<IAssetFilter>();

        public AssetGroup()
        {
            _id = IdentifierFactory.Create();
        }

        public string Id => _id;

        public IObservableProperty<string> Name => _name;

        /// <summary>
        ///     Filter to determine whether an asset belongs to this group.
        /// </summary>
        public IObservableList<IAssetFilter> Filters => _filters;

        public void Setup()
        {
            foreach (var filter in _filters)
                filter?.SetupForMatching();
        }

        public bool Validate(out string errorMessage)
        {
            var result = true;
            var sb = new StringBuilder();
            foreach (var filter in _filters)
            {
                result &= filter.Validate(out var message);
                if (!string.IsNullOrEmpty(message))
                    sb.AppendLine($"{Indent}{Indent}{message}");
            }

            if (sb.Length == 0)
            {
                errorMessage = null;
                return result;
            }

            errorMessage = sb.ToString();
            errorMessage = $"{Indent}Group: {_name.Value}{Environment.NewLine}{errorMessage}";
            return result;
        }

        /// <summary>
        ///     Return true if the asset belongs to this group.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        public bool Contains(string assetPath, Type assetType, bool isFolder)
        {
            if (_filters.Count == 0)
                return false;

            for (var i = 0; i < _filters.Count; i++)
            {
                var filter = _filters[i];
                if (filter == null)
                    continue;

                if (!filter.IsMatch(assetPath, assetType, isFolder))
                    return false;
            }

            return true;
        }

        public string GetDescription()
        {
            var result = new StringBuilder();
            var isFirstItem = true;
            foreach (var filter in _filters)
            {
                var description = filter.GetDescription();

                if (string.IsNullOrEmpty(description))
                    continue;

                if (!isFirstItem)
                    result.Append(" && ");

                result.Append(description);
                isFirstItem = false;
            }

            return result.ToString();
        }

        public void OverwriteValuesFromJson(string from)
        {
            var fromObj = JsonUtility.FromJson<AssetGroup>(from);
            _name.Value = fromObj._name.Value;
            _filters.Clear();

            foreach (var fromFilter in fromObj._filters)
            {
                var fromFilterJson = JsonUtility.ToJson(fromFilter);
                var filter = (IAssetFilter)Activator.CreateInstance(fromFilter.GetType());
                _filters.Add(filter);
                filter.OverwriteValuesFromJson(fromFilterJson);
            }
        }
    }
}
