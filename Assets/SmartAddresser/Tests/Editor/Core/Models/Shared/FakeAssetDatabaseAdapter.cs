using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;

namespace SmartAddresser.Tests.Editor.Core.Models.Shared
{
    internal sealed class FakeAssetDatabaseAdapter : IAssetDatabaseAdapter
    {
        private readonly List<Entry> _entries = new List<Entry>();

        public IList<Entry> Entries => _entries;

        public string[] GetAllAssetPaths()
        {
            return _entries.Select(x => x.AssetPath).ToArray();
        }

        public string GUIDToAssetPath(string guid)
        {
            var entry = _entries.FirstOrDefault(x => x.Guid == guid);
            return entry?.AssetPath;
        }

        public string AssetPathToGUID(string assetPath)
        {
            var entry = _entries.FirstOrDefault(x => x.AssetPath == assetPath);
            return entry?.Guid;
        }

        public Type GetMainAssetTypeAtPath(string assetPath)
        {
            var entry = _entries.FirstOrDefault(x => x.AssetPath == assetPath);
            return entry?.AssetType;
        }

        public bool IsValidFolder(string assetPath)
        {
            var entry = _entries.FirstOrDefault(x => x.AssetPath == assetPath);
            return entry?.IsValidFolder ?? false;
        }

        public sealed class Entry
        {
            public Entry(string guid, string assetPath, Type assetType, bool isValidFolder)
            {
                Guid = guid;
                AssetPath = assetPath;
                AssetType = assetType;
                IsValidFolder = isValidFolder;
            }

            public string Guid { get; }
            public string AssetPath { get; }
            public Type AssetType { get; }
            public bool IsValidFolder { get; }
        }
    }
}
