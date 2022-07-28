using System;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    public interface IProvider<out T>
    {
        void Setup();

        T Provide(string assetPath, Type assetType, bool isFolder);

        string GetDescription();
    }
}
