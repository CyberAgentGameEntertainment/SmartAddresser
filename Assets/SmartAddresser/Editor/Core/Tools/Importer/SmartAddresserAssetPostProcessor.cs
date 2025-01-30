using System.Linq;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation.AddressableAdapter;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using UnityEditor;
using UnityEditor.AddressableAssets;

namespace SmartAddresser.Editor.Core.Tools.Importer
{
    internal sealed class SmartAddresserAssetPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssetPaths,
            string[] deletedAssetPaths,
            string[] movedAssetPaths,
            string[] movedFromAssetPaths
        )
        {
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

            // Check this because AddressableAssetSettingsDefaultObject.Settings may be null at this point when the Library folder is deleted.
            if (addressableSettings == null)
                return;

            if (!ShouldProcess(importedAssetPaths, deletedAssetPaths, movedAssetPaths, movedFromAssetPaths))
                return;

            var layoutRuleDataRepository = new LayoutRuleDataRepository();
            var primaryData = layoutRuleDataRepository.PrimaryData;

            // If the primary data is not found, do nothing.
            if (primaryData == null)
                return;

            var layoutRules = layoutRuleDataRepository.PrimaryData.LayoutRules.ToArray();
            var versionExpressionParser = new VersionExpressionParserRepository().Load();
            var assetDatabaseAdapter = new AssetDatabaseAdapter();
            var addressableSettingsAdapter = new AddressableAssetSettingsAdapter(addressableSettings);
            var applyService = new ApplyLayoutRuleService(layoutRules,
                                                          versionExpressionParser,
                                                          addressableSettingsAdapter,
                                                          assetDatabaseAdapter);
            var validateLayoutRuleService = new ValidateAndExportLayoutRuleService(layoutRules);

            foreach (var layoutRule in layoutRules)
                layoutRule.Setup();

            // Check Layout Rule corruption
            var projectSettings = SmartAddresserProjectSettings.instance;
            var layoutRuleErrorHandleType = projectSettings.LayoutRuleErrorSettings.HandleType;
            validateLayoutRuleService.Execute(false, layoutRuleErrorHandleType, out _);

            // Apply
            foreach (var importedAssetPath in importedAssetPaths)
            {
                var guid = AssetDatabase.AssetPathToGUID(importedAssetPath);
                applyService.Apply(guid, false, true);
            }

            foreach (var movedAssetPath in movedAssetPaths)
            {
                var guid = AssetDatabase.AssetPathToGUID(movedAssetPath);
                applyService.Apply(guid, false, true);
            }

            applyService.InvokeBatchModificationEvent();
        }

        private static bool ShouldProcess(
            string[] importedAssetPaths,
            string[] deletedAssetPaths,
            string[] movedAssetPaths,
            string[] movedFromAssetPaths
        )
        {
            return importedAssetPaths
                .Concat(deletedAssetPaths)
                .Concat(movedAssetPaths)
                .Concat(movedFromAssetPaths)
                .Any(IsTarget);
        }

        private static bool IsTarget(string assetPath)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (type == typeof(LayoutRuleData))
                return false;

            return true;
        }
    }
}
