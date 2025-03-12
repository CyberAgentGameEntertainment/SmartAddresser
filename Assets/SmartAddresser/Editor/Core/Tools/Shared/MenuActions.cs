using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Foundation.AddressableAdapter;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using UnityEditor;
using UnityEditor.AddressableAssets;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    internal static class MenuActions
    {
        public static void ApplyAction(BaseLayoutRuleData target)
        {
            var projectSettings = SmartAddresserProjectSettings.instance;
            var primaryData = projectSettings.PrimaryData;

            if (primaryData == null)
            {
                Apply();
                return;
            }

            if (primaryData == target)
            {
                Apply();
                return;
            }

            // If the primary data is not the same as the editing data, ask the user to confirm.
            // If the user confirms, remove the primary data and apply the editing data.
            var dialogMessage =
                $"The {nameof(projectSettings.PrimaryData)} of the Project Settings is not the same as the data you are applying. Do you want to remove the {nameof(projectSettings.PrimaryData)} from Project Settings and apply the editing data?";
            if (EditorUtility.DisplayDialog("Confirm", dialogMessage, "Remove & Apply", "Cancel"))
            {
                projectSettings.PrimaryData = null;
                Apply();
            }

            return;

            void Apply()
            {
                var layoutRules = target.LayoutRules.ToArray();

                foreach (var layoutRule in layoutRules)
                    layoutRule.Setup();

                // Validate the layout rule.
                var validateService = new ValidateAndExportLayoutRuleService(layoutRules);
                var ruleErrorHandleType = projectSettings.LayoutRuleErrorSettings.HandleType;
                validateService.Execute(false, ruleErrorHandleType, out _);

                // Apply the layout rules to the addressable asset system.
                var versionExpressionParser = new VersionExpressionParserRepository().Load();
                var assetDatabaseAdapter = new AssetDatabaseAdapter();
                var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
                var addressableSettingsAdapter = new AddressableAssetSettingsAdapter(addressableSettings);
                var applyService = new ApplyLayoutRuleService(layoutRules,
                                                              versionExpressionParser,
                                                              addressableSettingsAdapter,
                                                              assetDatabaseAdapter);

                applyService.ApplyAll(false);
            }
        }
    }
}
