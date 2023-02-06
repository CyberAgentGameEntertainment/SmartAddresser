using SmartAddresser.Editor.Core.Models.LayoutRules;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    [FilePath("Smart Addresser/SmartAddresserSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class SmartAddresserProjectSettings : ScriptableSingleton<SmartAddresserProjectSettings>
    {
        [SerializeField] private LayoutRuleData primaryData;
        [SerializeField] private MonoScript versionExpressionParser;
        
        public LayoutRuleData PrimaryData
        {
            get => primaryData;
            set
            {
                if (value == primaryData)
                    return;

                primaryData = value;
                Save(true);
            }
        }
        
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
