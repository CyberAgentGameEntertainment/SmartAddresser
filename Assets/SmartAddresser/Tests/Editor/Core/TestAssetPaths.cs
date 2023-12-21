// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Linq;
using UnityEditor;

namespace SmartAddresser.Tests.Editor.Core
{
    public static class TestAssetRelativePaths
    {
        public const string PrefabDummy = "Dummy";
        public static class Shared
        {
            public const string Folder = "Shared";
            public const string Texture64 = Folder + "/tex_test_64.png";
            public const string Texture128 = Folder + "/tex_test_128.png";
            public const string Texture256 = Folder + "/tex_test_256.png";
            public const string MaterialTex64 = Folder + "/mat_test_tex_64.mat";
            public const string PrefabTex64 = Folder + "/prefab_test_tex_64.prefab";
        }

        public static class Dummy
        {
            public const string Folder = "Dummy";
            public const string PrefabDummy = Folder + "/prefab_dummy.prefab";
        }
        
        public static class Dummy1
        {
            public const string Folder = "Dummy1";
            public const string PrefabDummy = Folder + "/prefab_dummy_1.prefab";
        }
    }

    public static class TestAssetPaths
    {
        private static string _folder;

        public static string Folder
        {
            get
            {
                if (!string.IsNullOrEmpty(_folder))
                    return _folder;
                var asmdefGuid = AssetDatabase.FindAssets("SmartAddresser.Tests.Editor").First();
                var asmdefPath = AssetDatabase.GUIDToAssetPath(asmdefGuid);
                var asmdefFolderPath = asmdefPath.Substring(0, asmdefPath.LastIndexOf("/", StringComparison.Ordinal));
                var baseFolderPath = $"{asmdefFolderPath}/TestAssets";
                return baseFolderPath;
            }
        }

        public static string CreateAbsoluteAssetPath(string relativeAssetPath)
        {
            return $"{Folder}/{relativeAssetPath}";
        }

        public static string DummyPrefab => CreateAbsoluteAssetPath(TestAssetRelativePaths.PrefabDummy);
        public static class Shared
        {
            public static string Folder => CreateAbsoluteAssetPath(TestAssetRelativePaths.Shared.Folder);
            public static string Texture64 => CreateAbsoluteAssetPath(TestAssetRelativePaths.Shared.Texture64);
            public static string Texture128 => CreateAbsoluteAssetPath(TestAssetRelativePaths.Shared.Texture128);
            public static string Texture256 => CreateAbsoluteAssetPath(TestAssetRelativePaths.Shared.Texture256);
            public static string PrefabTex64 => CreateAbsoluteAssetPath(TestAssetRelativePaths.Shared.PrefabTex64);
            public static string MaterialTex64 => CreateAbsoluteAssetPath(TestAssetRelativePaths.Shared.MaterialTex64);
        }

        public static class Dummy
        {
            public static string Folder => CreateAbsoluteAssetPath(TestAssetRelativePaths.Dummy.Folder);
            public static string PrefabDummy => CreateAbsoluteAssetPath(TestAssetRelativePaths.Dummy.PrefabDummy);
        }

        public static class Dummy1
        {
            public static string Folder => CreateAbsoluteAssetPath(TestAssetRelativePaths.Dummy1.Folder);
            public static string PrefabDummy => CreateAbsoluteAssetPath(TestAssetRelativePaths.Dummy1.PrefabDummy);
        }
    }
}
