using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    [FilePath("Smart Addresser/SmartAddresserSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class SmartAddresserProjectSettings : ScriptableSingleton<SmartAddresserProjectSettings>
    {
        [SerializeField] private MonoScript versionExpressionParser;

        public MonoScript VersionExpressionParser
        {
            get => versionExpressionParser;
            set
            {
                if (value == versionExpressionParser)
                    return;

                versionExpressionParser = value;
                Save(true);
            }
        }
    }
}
