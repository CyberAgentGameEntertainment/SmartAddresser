// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.ListableProperty
{
    internal static class ListablePropertyEditorUtility
    {
        private static Texture _listIcon;

        public static Texture ListIcon
        {
            get
            {
                if (_listIcon == null)
                {
                    var texturePath = "tex_icon_listproperties_list";
                    texturePath += EditorGUIUtility.isProSkin ? "_proskin" : string.Empty;
                    _listIcon = Resources.Load<Texture>(texturePath);
                }

                return _listIcon;
            }
        }
    }
}
