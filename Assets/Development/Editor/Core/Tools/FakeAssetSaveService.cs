using System;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using Object = UnityEngine.Object;

namespace Development.Editor.Core.Tools
{
    internal sealed class FakeAssetSaveService : IAssetSaveService, IDisposable
    {
        private readonly ObservableProperty<bool> _isDirty;

        public FakeAssetSaveService()
        {
            _isDirty = new ObservableProperty<bool>(false);
        }

        public void SetAsset(Object asset)
        {
        }

        public Object Asset => null;
        public IReadOnlyObservableProperty<bool> IsDirty => _isDirty;

        public void Save()
        {
            MarkAsDirty();
            SaveImmediate();
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

        public void Dispose()
        {
            _isDirty.Dispose();
        }
    }
}
