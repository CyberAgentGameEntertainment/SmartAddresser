using SmartAddresser.Editor.Foundation;

namespace SmartAddresser.Editor.Core.Tools.CLI
{
    public sealed class ApplyRulesCLIOptions
    {
        private const string LayoutRuleAssetPathArgName = "-layoutRuleAssetPath";
        private const string ObsoleteShouldValidateArgName = "-validate"; // Keep for backward compatibility
        private const string ResultFilePathArgName = "-resultFilePath";
        private const string FailWhenWarningArgName = "-failWhenWarning";
        private const string DefaultResultFilePathWithoutExtensions = "SmartAddresser/validate_result";
        private const string ShouldValidateLayoutRuleArgName = "-validateLayoutRule";
        private const string ShouldValidateLayoutArgName = "-validateLayout";

        public string LayoutRuleAssetPath { get; private set; }
        public bool ShouldValidateLayout { get; private set; }
        public bool ShouldValidateLayoutRule { get; private set; }
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

            // Do Validate Layout
            var obsoleteShouldValidateLayout = CommandLineUtility.Contains(ObsoleteShouldValidateArgName);
            var newShouldValidateLayout = CommandLineUtility.Contains(ShouldValidateLayoutArgName);
            var shouldValidateLayout = obsoleteShouldValidateLayout || newShouldValidateLayout;
            options.ShouldValidateLayout = shouldValidateLayout;
            
            // Do Validate Layout Rule
            var shouldValidateLayoutRule = CommandLineUtility.Contains(ShouldValidateLayoutRuleArgName);
            options.ShouldValidateLayoutRule = shouldValidateLayoutRule;

            // Fail When Warning
            options.FailWhenWarning = CommandLineUtility.Contains(FailWhenWarningArgName);

            return options;
        }
    }
}
