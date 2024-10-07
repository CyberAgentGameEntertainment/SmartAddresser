using SmartAddresser.Editor.Foundation;

namespace SmartAddresser.Editor.Core.Tools.CLI
{
    public sealed class ValidateLayoutRuleCLIOptions
    {
        private const string LayoutRuleAssetPathArgName = "-layoutRuleAssetPath";
        private const string ErrorLogFilePathArgName = "-errorLogFilePath";

        public string LayoutRuleAssetPath { get; private set; }
        public string ErrorLogFilePath { get; private set; }

        public static ValidateLayoutRuleCLIOptions CreateFromCommandLineArgs()
        {
            var options = new ValidateLayoutRuleCLIOptions();

            // Layout Rule Asset Path
            if (CommandLineUtility.TryGetStringValue(LayoutRuleAssetPathArgName, out var layoutRuleAssetPath))
                options.LayoutRuleAssetPath = layoutRuleAssetPath;

            // Error Log File Path
            if (!CommandLineUtility.TryGetStringValue(ErrorLogFilePathArgName, out var errorLogFilePath))
                options.ErrorLogFilePath = errorLogFilePath;

            return options;
        }
    }
}
