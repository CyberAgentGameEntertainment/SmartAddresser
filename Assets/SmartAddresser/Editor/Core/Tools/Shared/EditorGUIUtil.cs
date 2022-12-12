// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    internal static class EditorGUIUtil
    {
        private static readonly Color BorderColorPro = new Color32(25, 25, 25, 255);

        private static readonly Color BorderColorNotPro = new Color32(138, 138, 138, 255);

        private static readonly Color TitleBackgroundColorPro = new Color32(62, 62, 62, 255);

        private static readonly Color TitleBackgroundColorNotPro = new Color32(203, 203, 203, 255);
        
        public static Color EditorBorderColor =>
            EditorGUIUtility.isProSkin ? BorderColorPro : BorderColorNotPro;

        public static Color TitleBackgroundColor =>
            EditorGUIUtility.isProSkin ? TitleBackgroundColorPro : TitleBackgroundColorNotPro;

        public const string ToolbarPlusIconName = "Toolbar Plus";
        public const string ToolbarMinusIconName = "Toolbar Minus";
        public const string MenuIconName = "d__Menu";
        public const string RefreshIconName = "d_Refresh";
    }
}
