using System;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    internal sealed class AssetSaveService : IAssetSaveService, IDisposable
    {
        private readonly ObservableProperty<bool> _isDirty = new ObservableProperty<bool>();
        private readonly Foundation.AssetSaveService _saveService = new Foundation.AssetSaveService();
        private bool _saveReserved;

        public AssetSaveService()
        {
            EditorApplication.update += OnUpdate;
        }

        public void SetAsset(Object asset)
        {
            // Save asset if dirty.
            if (EditorUtility.IsDirty(asset))
                _saveService.Run(asset);

            if (Asset != null && _saveReserved)
                SaveImmediate();

            _isDirty.Value = EditorUtility.IsDirty(asset);
            Asset = asset;
        }

        public Object Asset { get; private set; }

        public IReadOnlyObservableProperty<bool> IsDirty => _isDirty;

        public void Save()
        {
            if (Asset == null)
                return;

            MarkAsDirty();
            _saveReserved = true;
        }

        public void SaveImmediate()
        {
            if (Asset == null)
                return;

            MarkAsDirty();
            _saveService.Run(Asset);
        }

        public void MarkAsDirty()
        {
            if (Asset == null)
                return;

            EditorUtility.SetDirty(Asset);
        }

        public void ClearDirty()
        {
            if (Asset == null)
                return;

            EditorUtility.ClearDirty(Asset);
        }

        public void Dispose()
        {
            if (_saveReserved && Asset != null)
                SaveImmediate();

            _isDirty.Dispose();
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            if (Asset == null)
                return;

            CheckIsDirty();

            if (_saveReserved)
            {
                SaveImmediate();
                _saveReserved = false;
            }
        }

        private void CheckIsDirty()
        {
            if (Asset == null)
                return;

            var isDirty = EditorUtility.IsDirty(Asset);
            if (isDirty != _isDirty.Value)
                _isDirty.Value = isDirty;
        }
    }
}
