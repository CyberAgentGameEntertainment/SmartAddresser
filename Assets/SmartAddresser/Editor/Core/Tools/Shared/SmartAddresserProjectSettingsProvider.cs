using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Layouts;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    public sealed class SmartAddresserProjectSettingsProvider : SettingsProvider
    {
        public SmartAddresserProjectSettingsProvider(
            string path,
            SettingsScope scopes,
            IEnumerable<string> keywords = null
        )
            : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var keywords = new[] { "Smart Addresser" };
            return new SmartAddresserProjectSettingsProvider("Project/Smart Addresser",
                SettingsScope.Project,
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
                        (BaseLayoutRuleData)EditorGUILayout.ObjectField("Primary Data",
                                                                        projectSettings.PrimaryData,
                                                                        typeof(BaseLayoutRuleData),
                                                                        false);

                    if (ccs.changed && projectSettings.PrimaryData != null)
                    {
                        if (EditorUtility.DisplayDialog("Confirmation",
                            "If you change the Primary Data, it will be applied immediately. Do you want change it?",
                            "OK",
                            "Cancel"))
                        {
                            var layoutRules = projectSettings.PrimaryData.LayoutRules;
                            var validateService = new ValidateAndExportLayoutRuleService(layoutRules);

                            // Check Corruption
                            var corruptionNotificationType =
                                projectSettings.LayoutRuleErrorSettings.HandleType;
                            validateService.Execute(true, corruptionNotificationType, out _);
                        }
                        else
                        {
                            projectSettings.PrimaryData = oldData;
                        }
                    }
                }

                projectSettings.VersionExpressionParser =
                    (MonoScript)EditorGUILayout.ObjectField("Version Expression Parser",
                        projectSettings.VersionExpressionParser,
                        typeof(MonoScript),
                        false);

                EditorGUILayout.LabelField("Validation Settings");
                using (new EditorGUI.IndentLevelScope())
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    var duplicateAddresses = (EntryErrorType)EditorGUILayout.EnumPopup("Duplicate Addresses",
                        projectSettings.ValidationSettings.DuplicateAddresses);
                    var duplicateAssetPaths = (EntryErrorType)EditorGUILayout.EnumPopup("Duplicate Asset Paths",
                        projectSettings.ValidationSettings.DuplicateAssetPaths);
                    var entryHasMultipleVersions =
                        (EntryErrorType)EditorGUILayout.EnumPopup("Entry Has Multiple Versions",
                            projectSettings.ValidationSettings.EntryHasMultipleVersions);
                    if (ccs.changed)
                        projectSettings.ValidationSettings = new SmartAddresserProjectSettings.Validation(
                            duplicateAddresses,
                            duplicateAssetPaths,
                            entryHasMultipleVersions);
                }

                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    var notificationTypeOnApplyAll =
                        (LayoutRuleErrorHandleType)EditorGUILayout.EnumPopup(
                            "Layout Rule Error",
                            projectSettings.LayoutRuleErrorSettings.HandleType);
                    if (ccs.changed)
                        projectSettings.LayoutRuleErrorSettings =
                            new SmartAddresserProjectSettings.LayoutRuleError(notificationTypeOnApplyAll);
                }
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
