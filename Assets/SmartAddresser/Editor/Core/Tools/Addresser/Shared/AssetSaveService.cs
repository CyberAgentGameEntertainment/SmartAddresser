using System;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    internal sealed class AssetSaveService : IAssetSaveService, IDisposable
    {
        private readonly Foundation.AssetSaveService _saveService = new Foundation.AssetSaveService();

        private ObservableProperty<bool> _isDirty;
        private bool _saveReserved;

        public AssetSaveService()
        {
            EditorApplication.update += OnUpdate;
        }

        public void SetAsset(Object asset)
        {
            if (Asset != null)
            {
                if (_saveReserved)
                    SaveImmediate();

                _isDirty.Dispose();
            }
            _isDirty = new ObservableProperty<bool>(EditorUtility.IsDirty(asset));
            Asset = asset;
        }

        public Object Asset { get; private set; }

        public int SaveIntervalFrame { get; set; } = 10;

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
