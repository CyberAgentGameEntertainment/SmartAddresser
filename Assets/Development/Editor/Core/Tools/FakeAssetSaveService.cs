using System;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools
{
    internal sealed class FakeAssetSaveService : IAssetSaveService, IDisposable
    {
        private readonly ObservableProperty<bool> _isDirty;
        private bool _saveReserved;

        public FakeAssetSaveService()
        {
            _isDirty = new ObservableProperty<bool>(false);
            EditorApplication.update += OnUpdate;
        }

        public void Dispose()
        {
            if (_saveReserved)
                SaveImmediate();

            _isDirty.Dispose();
            EditorApplication.update -= OnUpdate;
        }

        public int SaveIntervalFrame { get; set; } = 10;

        public IReadOnlyObservableProperty<bool> IsDirty => _isDirty;

        public void Save()
        {
            MarkAsDirty();
            _saveReserved = true;
        }

        public void SaveImmediate()
        {
            _isDirty.Value = false;
        }

        public void MarkAsDirty()
        {
            _isDirty.Value = true;
        }

        public void ClearDirty()
        {
            _isDirty.Value = false;
        }

        private void OnUpdate()
        {
            if (_saveReserved && Time.frameCount % SaveIntervalFrame == 0)
            {
                SaveImmediate();
                _saveReserved = false;
            }
        }
    }
}
