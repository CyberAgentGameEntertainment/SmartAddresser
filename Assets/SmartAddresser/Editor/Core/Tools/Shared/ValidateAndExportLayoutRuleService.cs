// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly ValidateLayoutRuleService[] _validateServices;

        public ValidateAndExportLayoutRuleService(LayoutRule layoutRule, string overrideExportFilePath = null) :
            this(new[] { layoutRule }, overrideExportFilePath)
        {
        }

        public ValidateAndExportLayoutRuleService(IEnumerable<LayoutRule> layoutRules,
                                                  string overrideExportFilePath = null)
        {
            _validateServices = layoutRules.Select(rule => new ValidateLayoutRuleService(rule)).ToArray();
            _exportService = new ExportLayoutRuleValidationErrorService();
            _exportFilePath = string.IsNullOrEmpty(overrideExportFilePath)
                                  ? DefaultExportFilePath
                                  : overrideExportFilePath;
        }

        public bool Execute(
            bool doSetup,
            LayoutRuleErrorHandleType handleType,
            out List<LayoutRuleValidationError> errorList
        )
        {
            errorList = new List<LayoutRuleValidationError>(_validateServices.Length);

            if (handleType == LayoutRuleErrorHandleType.Ignore)
                return false;

            // Validate
            var exportFileName = Path.GetFileNameWithoutExtension(_exportFilePath);
            var exportFileExtension = Path.GetExtension(_exportFilePath);
            for (var i = 0; i < _validateServices.Length; i++)
            {
                var validateService = _validateServices[i];
                if (validateService.Execute(doSetup, out var error))
                    continue;

                errorList.Add(error);
                // Export
                var exportFilePath = $"{exportFileName}_{i}{exportFileExtension}";
                _exportService.Run(error, exportFilePath);
            }

            if (errorList.Count == 0)
                return true;

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
