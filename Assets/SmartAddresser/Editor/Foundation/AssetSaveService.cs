using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SmartAddresser.Editor.Foundation
{
    /// <summary>
    ///     Service to save a single asset.
    /// </summary>
    internal class AssetSaveService : UnityEditor.AssetModificationProcessor
    {
        private static readonly List<string> _paths = new List<string>();

        private static string[] OnWillSaveAssets(string[] paths)
        {
            if (paths.Length == 0) return paths;

            if (_paths.Count == 0) return paths;

            return _paths.ToArray();
        }

        public void Run(Object obj)
        {
            if (!EditorUtility.IsDirty(obj)) return;

            var path = AssetDatabase.GetAssetPath(obj);
            _paths.Add(path);
            AssetDatabase.SaveAssets();
            _paths.Clear();
        }

        public void Run(IEnumerable<Object> objs)
        {
            foreach (var obj in objs)
            {
                if (!EditorUtility.IsDirty(obj)) continue;

                var path = AssetDatabase.GetAssetPath(obj);

                if (string.IsNullOrEmpty(path))
                    throw new InvalidOperationException($"{nameof(obj)} is not valid asset.");

                _paths.Add(path);
                AssetDatabase.SaveAssets();
                _paths.Clear();
            }
        }
    }
}
