// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups
{
    public interface IAssetFilter
    {
        string Id { get; }

        /// <summary>
        ///     Preprocessing of <see cref="IsMatch" />.
        /// </summary>
        /// <remarks>
        ///     This will be called before and less often than <see cref="IsMatch" />.
        ///     And will be executed in the main thread, in contrast to <see cref="IsMatch" />.
        ///     So you should write heavy processes or processes that use Unity's API here.
        /// </remarks>
        void SetupForMatching();

        /// <summary>
        ///     Returns false if the AssetFilter is corrupted.
        ///     Example: When an asset referenced by the filter is deleted.
        ///     This method will be called after <see cref="SetupForMatching"/>.
        /// </summary>
        /// <returns></returns>
        bool Validate(out string errorMessage);

        /// <summary>
        ///     Return true if the asset passes this filter.
        /// </summary>
        /// <remarks>
        ///     This may be executed outside the main thread.
        /// </remarks>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        bool IsMatch(string assetPath, Type assetType, bool isFolder);

        /// <summary>
        ///     Get a description of this asset filter.
        /// </summary>
        /// <returns></returns>
        string GetDescription();

        void OverwriteValuesFromJson(string json);
    }
}
