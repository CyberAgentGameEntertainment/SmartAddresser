// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Shared
{
    public sealed class ValidateAndExportLayoutRuleService
    {
        public const string DefaultExportFilePath = "Logs/SmartAddresser_LayoutRuleError.json";
        private readonly string _exportFilePath;

        private readonly ExportLayoutRuleValidationErrorService _exportService;
        private readonly ValidateLayoutRuleService _validateService;

        public ValidateAndExportLayoutRuleService(LayoutRule layoutRule, string overrideExportFilePath = null)
        {
            _validateService = new ValidateLayoutRuleService(layoutRule);
            _exportService = new ExportLayoutRuleValidationErrorService();
            _exportFilePath = string.IsNullOrEmpty(overrideExportFilePath)
                ? DefaultExportFilePath
                : overrideExportFilePath;
        }

        public bool Execute(
            bool doSetup,
            LayoutRuleErrorHandleType handleType,
            out LayoutRuleValidationError error
        )
        {
            if (handleType == LayoutRuleErrorHandleType.Ignore)
            {
                error = null;
                return false;
            }

            // Validate
            if (_validateService.Execute(doSetup, out error))
                return true;

            // Export
            _exportService.Run(error, _exportFilePath);

            // Log / Exception
            var message =
                $"[Smart Addresser] There are errors in the layout rule. Please check {_exportFilePath} for details.";
            if (handleType == LayoutRuleErrorHandleType.LogError)
                Debug.LogError(message);
            else if (handleType == LayoutRuleErrorHandleType.ThrowException)
                throw new Exception(message);

            return false;
        }
    }
}
