using System;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    internal sealed class AssetSaveService : IAssetSaveService, IDisposable
    {
        private readonly ObservableProperty<bool> _isDirty;
        private readonly Foundation.AssetSaveService _saveService = new Foundation.AssetSaveService();

        private bool _saveReserved;

        public AssetSaveService(Object asset)
        {
            Asset = asset;
            _isDirty = new ObservableProperty<bool>(EditorUtility.IsDirty(asset));
            EditorApplication.update += OnUpdate;
        }

        public Object Asset { get; }

        public int SaveIntervalFrame { get; set; } = 10;

        public IReadOnlyObservableProperty<bool> IsDirty => _isDirty;

        public void Save()
        {
            MarkAsDirty();
            _saveReserved = true;
        }

        public void SaveImmediate()
        {
            MarkAsDirty();
            _saveService.Run(Asset);
        }

        public void MarkAsDirty()
        {
            EditorUtility.SetDirty(Asset);
        }

        public void ClearDirty()
        {
            EditorUtility.ClearDirty(Asset);
        }

        public void Dispose()
        {
            if (_saveReserved)
                SaveImmediate();

            _isDirty.Dispose();
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            CheckIsDirty();

            if (_saveReserved && Time.frameCount % SaveIntervalFrame == 0)
            {
                SaveImmediate();
                _saveReserved = false;
            }
        }

        private void CheckIsDirty()
        {
            var isDirty = EditorUtility.IsDirty(Asset);
            if (isDirty != _isDirty.Value)
                _isDirty.Value = isDirty;
        }
    }
}
