using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Foundation.AddressableAdapter;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using UnityEditor;
using UnityEditor.AddressableAssets;
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

                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    var oldData = projectSettings.PrimaryData;
                    projectSettings.PrimaryData =
                        (LayoutRuleData)EditorGUILayout.ObjectField("Primary Data",
                            projectSettings.PrimaryData,
                            typeof(LayoutRuleData), false);

                    if (ccs.changed && projectSettings.PrimaryData != null)
                    {
                        if (EditorUtility.DisplayDialog("Confirmation",
                                "If you change the Primary Data, it will be applied immediately. Do you want change it?",
                                "OK", "Cancel"))
                        {
                            var layoutRule = projectSettings.PrimaryData.LayoutRule;
                            var versionExpressionParser = new VersionExpressionParserRepository().Load();
                            var assetDatabaseAdapter = new AssetDatabaseAdapter();
                            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
                            var addressableSettingsAdapter = new AddressableAssetSettingsAdapter(addressableSettings);
                            var applyService = new ApplyLayoutRuleService(layoutRule, versionExpressionParser,
                                addressableSettingsAdapter, assetDatabaseAdapter);
                            applyService.UpdateAllEntries();
                        }
                        else
                        {
                            projectSettings.PrimaryData = oldData;
                        }
                    }
                }

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
