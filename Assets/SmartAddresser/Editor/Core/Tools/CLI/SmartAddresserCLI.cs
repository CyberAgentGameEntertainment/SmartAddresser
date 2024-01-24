using System;
using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Layouts;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation;
using SmartAddresser.Editor.Foundation.AddressableAdapter;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.CLI
{
    public static class SmartAddresserCLI
    {
        private const int ErrorLevelNone = 0;
        private const int ErrorLevelValidateFailed = 1;
        private const int ErrorLevelFailed = 2;

        public static void SetVersionExpression()
        {
            try
            {
                var options = SetVersionExpressionCLIOptions.CreateFromCommandLineArgs();

                var layoutRuleData = LoadLayoutRuleData(options.LayoutRuleAssetPath);
                var layoutRule = layoutRuleData.LayoutRule;
                layoutRule.Settings.VersionExpression.Value = options.VersionExpression;

                // Save the LayoutRuleData asset.
                EditorUtility.SetDirty(layoutRuleData);
                var assetSaveService = new AssetSaveService();
                assetSaveService.Run(layoutRuleData);

                EditorApplication.Exit(ErrorLevelNone);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorApplication.Exit(ErrorLevelFailed);
            }
        }

        public static void ApplyRules()
        {
            try
            {
                var projectSettings = SmartAddresserProjectSettings.instance;
                var validationSettings = projectSettings.ValidationSettings;
                var options = ApplyRulesCLIOptions.CreateFromCommandLineArgs();
                var layoutRule = LoadLayoutRuleData(options.LayoutRuleAssetPath).LayoutRule;
                var versionExpressionParser = new VersionExpressionParserRepository().Load();
                var assetDatabaseAdapter = new AssetDatabaseAdapter();
                var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
                var addressableSettingsAdapter = new AddressableAssetSettingsAdapter(addressableSettings);

                if (options.ShouldValidate)
                {
                    // Build and validate the Layout.
                    var buildLayoutService = new BuildLayoutService(assetDatabaseAdapter);
                    var layout = buildLayoutService.Execute(layoutRule);
                    layout.Validate(true, validationSettings.DuplicateAddresses, validationSettings.DuplicateAssetPaths,
                        validationSettings.EntryHasMultipleVersions);

                    // Export the result of the validation.
                    var validateResultExportService = new ValidateResultExportService(layout);
                    validateResultExportService.Run(options.ResultFilePath);

                    // Exit if error occurred.
                    if (layout.ErrorType == LayoutErrorType.Error
                        || (options.FailWhenWarning && layout.ErrorType == LayoutErrorType.Warning))
                    {
                        EditorApplication.Exit(ErrorLevelValidateFailed);
                        return;
                    }
                }

                // Apply the layout rules to the addressable asset system.
                var applyService = new ApplyLayoutRuleService(layoutRule, versionExpressionParser,
                    addressableSettingsAdapter, assetDatabaseAdapter);
                applyService.ApplyAll();

                EditorApplication.Exit(ErrorLevelNone);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorApplication.Exit(ErrorLevelFailed);
            }
        }

        private static LayoutRuleData LoadLayoutRuleData(string assetPath = null)
        {
            if (!string.IsNullOrEmpty(assetPath))
                return AssetDatabase.LoadAssetAtPath<LayoutRuleData>(assetPath);

            var guid = AssetDatabase.FindAssets($"t: {nameof(LayoutRuleData)}").FirstOrDefault();
            if (string.IsNullOrEmpty(guid))
                throw new InvalidOperationException("There is no LayoutRuleData in the project.");

            assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<LayoutRuleData>(assetPath);
        }
    }
}
