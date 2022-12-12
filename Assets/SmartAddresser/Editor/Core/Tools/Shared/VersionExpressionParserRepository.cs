using System;
using SmartAddresser.Editor.Foundation.SemanticVersioning;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    public sealed class VersionExpressionParserRepository
    {
        public IVersionExpressionParser Load()
        {
            var settings = SmartAddresserProjectSettings.instance;

            if (settings.VersionExpressionParser == null)
                return new UnityVersionExpressionParser();

            return (IVersionExpressionParser)Activator.CreateInstance(settings.VersionExpressionParser.GetClass());
        }
    }
}
