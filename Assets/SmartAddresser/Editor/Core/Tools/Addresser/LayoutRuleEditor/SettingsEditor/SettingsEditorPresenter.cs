using System;
using SmartAddresser.Editor.Core.Models.LayoutRules.Settings;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using StateBasedHistory = SmartAddresser.Editor.Foundation.StateBasedUndo.History;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.SettingsEditor
{
    /// <summary>
    ///     Presenter for the <see cref="SettingsEditorView" />.
    /// </summary>
    internal sealed class SettingsEditorPresenter : IDisposable
    {
        private readonly IAssetSaveService _assetSaveService;
        private readonly AutoIncrementHistory _history;
        private readonly CompositeDisposable _setupViewDisposables = new CompositeDisposable();
        private readonly SettingsEditorView _view;
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();
        private bool _didSetupView;

        private LayoutRuleSettings _layoutRuleSettings;

        public SettingsEditorPresenter(SettingsEditorView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _history = history;
            _assetSaveService = saveService;

            SetupViewEventHandlers();
        }

        public void Dispose()
        {
            CleanupView();
            CleanupViewEventHandlers();
        }

        public void SetupView(LayoutRuleSettings settings)
        {
            CleanupView();

            _layoutRuleSettings = settings;
            _view.VersionExpression.SetValueAndNotNotify(settings.VersionExpression.Value);

            _view.Enabled = true;
            _didSetupView = true;
        }

        public void CleanupView()
        {
            _setupViewDisposables.Clear();
            _view.VersionExpression.SetValueAndNotNotify(string.Empty);
            _view.Enabled = false;
            _didSetupView = false;
            _layoutRuleSettings = null;
        }

        private void SetupViewEventHandlers()
        {
            _view.VersionExpression.Subscribe(OnVersionExpressionChanged)
                .DisposeWith(_viewEventDisposables);
            _view.ExcludeUnversioned.Subscribe(OnExcludeUnversionedChanged)
                .DisposeWith(_viewEventDisposables);

            #region Local methods

            void OnVersionExpressionChanged(string versionExpression)
            {
                if (!_didSetupView)
                    return;

                var oldVersionExpression = _layoutRuleSettings.VersionExpression.Value;
                _history.Register($"Change Version Expression {versionExpression}", () =>
                {
                    _layoutRuleSettings.VersionExpression.Value = versionExpression;
                    _assetSaveService.Save();
                }, () =>
                {
                    _layoutRuleSettings.VersionExpression.Value = oldVersionExpression;
                    _assetSaveService.Save();
                });
            }

            void OnExcludeUnversionedChanged(bool excludeUnversioned)
            {
                if (!_didSetupView)
                    return;

                var oldExcludeUnversioned = _layoutRuleSettings.ExcludeUnversioned.Value;
                _history.Register($"Change Version Expression {excludeUnversioned}", () =>
                {
                    _layoutRuleSettings.ExcludeUnversioned.Value = excludeUnversioned;
                    _assetSaveService.Save();
                }, () =>
                {
                    _layoutRuleSettings.ExcludeUnversioned.Value = oldExcludeUnversioned;
                    _assetSaveService.Save();
                });
            }

            #endregion
        }

        private void CleanupViewEventHandlers()
        {
            _viewEventDisposables.Clear();
        }
    }
}
