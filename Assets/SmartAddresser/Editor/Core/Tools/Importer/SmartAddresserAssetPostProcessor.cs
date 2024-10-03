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
            // Delay 1 frame because AddressableAssetSettingsDefaultObject.Settings may be null at this point when the Library folder is deleted.
            EditorApplication.delayCall += () =>
                LazyOnPostprocessAllAssets(importedAssetPaths, deletedAssetPaths, movedAssetPaths, movedFromAssetPaths);
        }

        private static void LazyOnPostprocessAllAssets(
            string[] importedAssetPaths,
            string[] deletedAssetPaths,
            string[] movedAssetPaths,
            string[] movedFromAssetPaths
        )
        {
            var layoutRuleDataRepository = new LayoutRuleDataRepository();
            var primaryData = layoutRuleDataRepository.PrimaryData;

            // If the primary data is not found, do nothing.
            if (primaryData == null)
                return;

            var layoutRule = layoutRuleDataRepository.PrimaryData.LayoutRule;
            var versionExpressionParser = new VersionExpressionParserRepository().Load();
            var assetDatabaseAdapter = new AssetDatabaseAdapter();
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            var addressableSettingsAdapter = new AddressableAssetSettingsAdapter(addressableSettings);
            var applyService = new ApplyLayoutRuleService(layoutRule,
                versionExpressionParser,
                addressableSettingsAdapter,
                assetDatabaseAdapter);
            applyService.Setup();

            // Check Corruption
            var projectSettings = SmartAddresserProjectSettings.instance;
            var corruptionNotificationType = projectSettings.LayoutRuleCorruptionSettings.NotificationTypeOnImport;
            applyService.ValidateLayoutRules(corruptionNotificationType);

            var versionExpression = layoutRule.Settings.VersionExpression.Value;
            if (string.IsNullOrEmpty(versionExpression))
                versionExpression = null;

            foreach (var importedAssetPath in importedAssetPaths)
            {
                var guid = AssetDatabase.AssetPathToGUID(importedAssetPath);
                applyService.Apply(guid, false, true, versionExpression);
            }

            foreach (var movedAssetPath in movedAssetPaths)
            {
                var guid = AssetDatabase.AssetPathToGUID(movedAssetPath);
                applyService.Apply(guid, false, true, versionExpression);
            }

            applyService.InvokeBatchModificationEvent();
        }
    }
}
