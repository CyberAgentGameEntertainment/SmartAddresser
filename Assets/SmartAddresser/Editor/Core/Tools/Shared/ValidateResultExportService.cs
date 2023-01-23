using System.IO;
using SmartAddresser.Editor.Core.Models.Layouts;
using UnityEngine;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    public sealed class ValidateResultExportService
    {
        private readonly Layout _layout;

        public ValidateResultExportService(Layout layout)
        {
            _layout = layout;
        }

        public void Run(string filePath)
        {
            Assert.IsTrue(_layout.HasValidated);

            var json = JsonUtility.ToJson(_layout, true);
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
