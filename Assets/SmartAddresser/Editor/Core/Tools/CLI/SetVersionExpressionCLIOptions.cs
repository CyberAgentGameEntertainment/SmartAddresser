using SmartAddresser.Editor.Foundation;

namespace SmartAddresser.Editor.Core.Tools.CLI
{
    public sealed class SetVersionExpressionCLIOptions
    {
        private const string LayoutRuleAssetPathArgName = "-layoutRuleAssetPath";
        private const string VersionExpressionArgName = "-versionExpression";

        public string LayoutRuleAssetPath { get; private set; }
        public string VersionExpression { get; private set; }

        public static SetVersionExpressionCLIOptions CreateFromCommandLineArgs()
        {
            var options = new SetVersionExpressionCLIOptions();

            // Layout Rule Asset Path
            if (CommandLineUtility.TryGetStringValue(LayoutRuleAssetPathArgName, out var layoutRuleAssetPath))
                options.LayoutRuleAssetPath = layoutRuleAssetPath;

            // Version Expression
            if (CommandLineUtility.TryGetStringValue(VersionExpressionArgName, out var versionExpression))
                options.VersionExpression = versionExpression;

            return options;
        }
    }
}
