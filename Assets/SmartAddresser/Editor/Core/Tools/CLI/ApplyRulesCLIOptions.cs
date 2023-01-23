using SmartAddresser.Editor.Foundation;

namespace SmartAddresser.Editor.Core.Tools.CLI
{
    public sealed class ApplyRulesCLIOptions
    {
        private const string LayoutRuleAssetPathArgName = "-layoutRuleAssetPath";
        private const string ShouldValidateArgName = "-validate";
        private const string ResultFilePathArgName = "-resultFilePath";
        private const string FailWhenWarningArgName = "-failWhenWarning";
        private const string DefaultResultFilePathWithoutExtensions = "SmartAddresser/validate_result";

        public string LayoutRuleAssetPath { get; private set; }
        public bool ShouldValidate { get; private set; }
        public string ResultFilePath { get; private set; }
        public bool FailWhenWarning { get; private set; }

        public static ApplyRulesCLIOptions CreateFromCommandLineArgs()
        {
            var options = new ApplyRulesCLIOptions();

            // Layout Rule Asset Path
            if (CommandLineUtility.TryGetStringValue(LayoutRuleAssetPathArgName, out var layoutRuleAssetPath))
                options.LayoutRuleAssetPath = layoutRuleAssetPath;

            // Result File Path
            if (!CommandLineUtility.TryGetStringValue(ResultFilePathArgName, out var resultFilePath))
                resultFilePath = $"{DefaultResultFilePathWithoutExtensions}.json";
            options.ResultFilePath = resultFilePath;

            // Do Validate
            options.ShouldValidate = CommandLineUtility.Contains(ShouldValidateArgName);

            // Fail When Warning
            options.FailWhenWarning = CommandLineUtility.Contains(FailWhenWarningArgName);

            return options;
        }
    }
}
