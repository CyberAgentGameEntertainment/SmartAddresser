using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    public sealed class SmartAddresserProjectSettingsProvider : SettingsProvider
    {
        public SmartAddresserProjectSettingsProvider(string path, SettingsScope scopes,
            IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var keywords = new[] { "Smart Addresser" };
            return new SmartAddresserProjectSettingsProvider("Project/Smart Addresser", SettingsScope.Project,
                keywords);
        }

        public override void OnGUI(string searchContext)
        {
            using (new GUIScope())
            {
                var projectSettings = SmartAddresserProjectSettings.instance;

                projectSettings.VersionExpressionParser =
                    (MonoScript)EditorGUILayout.ObjectField("Version Expression Paraser",
                        projectSettings.VersionExpressionParser,
                        typeof(MonoScript), false);
            }
        }

        private sealed class GUIScope : GUI.Scope
        {
            private const float LabelWidth = 250;
            private const float MarginLeft = 10;
            private const float MarginTop = 10;

            private readonly float _labelWidth;

            public GUIScope()
            {
                _labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = LabelWidth;
                GUILayout.BeginHorizontal();
                GUILayout.Space(MarginLeft);
                GUILayout.BeginVertical();
                GUILayout.Space(MarginTop);
            }

            protected override void CloseScope()
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = _labelWidth;
            }
        }
    }
}
