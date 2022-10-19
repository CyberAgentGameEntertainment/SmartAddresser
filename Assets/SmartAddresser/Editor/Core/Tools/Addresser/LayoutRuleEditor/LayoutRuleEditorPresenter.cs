using System;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.LabelRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.VersionRuleEditor;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="LayoutRuleEditorView" />.
    /// </summary>
    internal sealed class LayoutRuleEditorPresenter : IDisposable
    {
        private readonly AddressRuleEditorPresenter _addressRuleEditorPresenter;
        private readonly LabelRuleEditorPresenter _labelRuleEditorPresenter;
        private readonly VersionRuleEditorPresenter _versionRuleEditorPresenter;
        private readonly LayoutRuleEditorView _view;
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();

        public LayoutRuleEditorPresenter(LayoutRuleEditorView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _addressRuleEditorPresenter =
                new AddressRuleEditorPresenter(view.AddressRuleEditorView, history, saveService);
            _labelRuleEditorPresenter = new LabelRuleEditorPresenter(view.LabelRuleEditorView, history, saveService);
            _versionRuleEditorPresenter =
                new VersionRuleEditorPresenter(view.VersionRuleEditorView, history, saveService);

            SetupViewEventHandlers();
        }

        public void Dispose()
        {
            _addressRuleEditorPresenter.Dispose();
            _labelRuleEditorPresenter.Dispose();
            _versionRuleEditorPresenter.Dispose();
            CleanupView();
            CleanupViewEventHandlers();
        }

        public void SetupView(LayoutRule layoutRule)
        {
            CleanupView();

            _addressRuleEditorPresenter.SetupView(layoutRule.AddressRules);
            _labelRuleEditorPresenter.SetupView(layoutRule.LabelRules);
            _versionRuleEditorPresenter.SetupView(layoutRule.VersionRules);
        }

        public void CleanupView()
        {
            _addressRuleEditorPresenter.CleanupView();
            _labelRuleEditorPresenter.CleanupView();
            _versionRuleEditorPresenter.CleanupView();
        }

        private void SetupViewEventHandlers()
        {
            _view.ApplyButtonClickedAsObservable.Subscribe(x =>
            {
                // TODO: ルールを適用する仕組みを実装後、適用処理を書く
            }).DisposeWith(_viewEventDisposables);
        }

        private void CleanupViewEventHandlers()
        {
            _viewEventDisposables.Clear();
        }
    }
}
