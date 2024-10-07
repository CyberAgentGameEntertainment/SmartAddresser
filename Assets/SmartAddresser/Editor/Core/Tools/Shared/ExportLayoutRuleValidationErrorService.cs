using System.IO;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    public sealed class ExportLayoutRuleValidationErrorService
    {
        public void Run(LayoutRuleValidationError error, string filePath)
        {
            var json = JsonUtility.ToJson(error, true);
            ExportText(json, filePath);
        }

        private static void ExportText(string text, string filePath)
        {
            var folderPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            File.WriteAllText(filePath, text);
        }
    }
}
