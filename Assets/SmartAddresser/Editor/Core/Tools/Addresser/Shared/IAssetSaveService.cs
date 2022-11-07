using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    /// <summary>
    ///     Interface for the service to save the asset.
    /// </summary>
    internal interface IAssetSaveService
    {
        /// <summary>
        ///     Target asset.
        /// </summary>
        public Object Asset { get; }

        IReadOnlyObservableProperty<bool> IsDirty { get; }

        /// <summary>
        ///     Save the asset.
        ///     It may take several frames to actually be saved.
        ///     If you want to save the asset immediately, use <see cref="SaveImmediate" />.
        /// </summary>
        void Save();

        /// <summary>
        ///     Save the asset immediately.
        /// </summary>
        void SaveImmediate();

        /// <summary>
        ///     Mark the asset dirty.
        /// </summary>
        void MarkAsDirty();

        /// <summary>
        ///     Clear the dirty flag.
        /// </summary>
        void ClearDirty();

        void SetAsset(Object asset);
    }
}
