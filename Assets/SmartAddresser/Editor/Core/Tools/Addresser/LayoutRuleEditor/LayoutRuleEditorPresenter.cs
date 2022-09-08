using System;
using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor;
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
        private readonly CompositeDisposable _viewEventDisposables = new CompositeDisposable();
        private readonly LayoutRuleEditorView _view;

        public LayoutRuleEditorPresenter(LayoutRuleEditorView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
            _addressRuleEditorPresenter =
                new AddressRuleEditorPresenter(view.AddressRuleEditorView, history, saveService);
            
            SetupViewEventHandlers();
        }

        public void Dispose()
        {
            _addressRuleEditorPresenter.Dispose();
            CleanupView();
            CleanupViewEventHandlers();
        }

        public void SetupView(LayoutRule layoutRule)
        {
            CleanupView();

            _addressRuleEditorPresenter.SetupView(layoutRule.AddressRules);
        }

        public void CleanupView()
        {
            _addressRuleEditorPresenter.CleanupView();
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
