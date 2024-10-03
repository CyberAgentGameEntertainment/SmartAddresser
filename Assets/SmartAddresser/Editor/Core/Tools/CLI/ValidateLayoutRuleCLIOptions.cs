using SmartAddresser.Editor.Foundation;

namespace SmartAddresser.Editor.Core.Tools.CLI
{
    public sealed class ValidateLayoutRuleCLIOptions
    {
        private const string LayoutRuleAssetPathArgName = "-layoutRuleAssetPath";

        public string LayoutRuleAssetPath { get; private set; }

        public static ValidateLayoutRuleCLIOptions CreateFromCommandLineArgs()
        {
            var options = new ValidateLayoutRuleCLIOptions();

            // Layout Rule Asset Path
            if (CommandLineUtility.TryGetStringValue(LayoutRuleAssetPathArgName, out var layoutRuleAssetPath))
                options.LayoutRuleAssetPath = layoutRuleAssetPath;

            return options;
        }
    }
}
