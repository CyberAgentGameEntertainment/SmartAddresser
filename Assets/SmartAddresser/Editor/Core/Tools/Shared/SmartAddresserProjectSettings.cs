using System;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Layouts;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    [FilePath("Smart Addresser/SmartAddresserSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class SmartAddresserProjectSettings : ScriptableSingleton<SmartAddresserProjectSettings>
    {
        [SerializeField] private BaseLayoutRuleData primaryData;
        [SerializeField] private MonoScript versionExpressionParser;
        [SerializeField] private Validation validation = new Validation();
        [SerializeField] private LayoutRuleError layoutRuleError = new LayoutRuleError();

        public BaseLayoutRuleData PrimaryData
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

        public Validation ValidationSettings
        {
            get => validation;
            set
            {
                if (value == validation)
                    return;

                validation = value;
                Save(true);
            }
        }

        public LayoutRuleError LayoutRuleErrorSettings
        {
            get => layoutRuleError;
            set
            {
                if (value == layoutRuleError)
                    return;

                layoutRuleError = value;
                Save(true);
            }
        }

        [Serializable]
        public sealed class Validation
        {
            [SerializeField] private EntryErrorType duplicateAddresses = EntryErrorType.Warning;
            [SerializeField] private EntryErrorType duplicateAssetPaths = EntryErrorType.Error;
            [SerializeField] private EntryErrorType entryHasMultipleVersions = EntryErrorType.Error;

            public Validation()
            {
            }

            public Validation(
                EntryErrorType duplicateAddresses,
                EntryErrorType duplicateAssetPaths,
                EntryErrorType entryHasMultipleVersions
            )
            {
                this.duplicateAddresses = duplicateAddresses;
                this.duplicateAssetPaths = duplicateAssetPaths;
                this.entryHasMultipleVersions = entryHasMultipleVersions;
            }

            public EntryErrorType DuplicateAddresses => duplicateAddresses;
            public EntryErrorType DuplicateAssetPaths => duplicateAssetPaths;
            public EntryErrorType EntryHasMultipleVersions => entryHasMultipleVersions;
        }

        [Serializable]
        public sealed class LayoutRuleError
        {
            [SerializeField] private LayoutRuleErrorHandleType handleType =
                LayoutRuleErrorHandleType.LogError;

            public LayoutRuleError()
            {
            }

            public LayoutRuleError(
                LayoutRuleErrorHandleType handleType
            )
            {
                this.handleType = handleType;
            }

            public LayoutRuleErrorHandleType HandleType => handleType;
        }
    }
}
