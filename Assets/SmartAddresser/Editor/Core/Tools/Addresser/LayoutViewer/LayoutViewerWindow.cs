using SmartAddresser.Editor.Core.Models.LayoutRules;
using SmartAddresser.Editor.Core.Models.Services;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
using SmartAddresser.Editor.Foundation.EditorSplitView;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer
{
    internal sealed class LayoutViewerWindow : EditorWindow
    {
        private const string WindowName = "Layout Viewer";

        [SerializeField] private LayoutViewerTreeView.State _treeViewState = new LayoutViewerTreeView.State();
        [SerializeField] private string _searchFieldText;
        [SerializeField] private EditorGUILayoutSplitViewState _splitViewState;
        private readonly CompositeDisposable _setupDisposables = new CompositeDisposable();

        private LayoutViewerPresenter _presenter;
        private LayoutViewerView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);
            Setup();
        }

        private void OnDisable()
        {
            _presenter?.Dispose();
            _view?.Dispose();
        }

        private void OnDestroy()
        {
            _setupDisposables.Dispose();
        }

        private void OnGUI()
        {
            _view.DoLayout();
        }

        private void Setup(LayoutRuleData initialData = null)
        {
            _setupDisposables.Clear();
            _presenter?.Dispose();
            _view?.Dispose();

            if (_splitViewState == null)
                _splitViewState = new EditorGUILayoutSplitViewState(LayoutDirection.Vertical, 0.75f);

            var buildLayoutService = new BuildLayoutService(new AssetDatabaseAdapter());
            _view = new LayoutViewerView(_treeViewState, _splitViewState, _searchFieldText, Repaint);
            _view.SearchText
                .Subscribe(x => _searchFieldText = x)
                .DisposeWith(_setupDisposables);
            _presenter = new LayoutViewerPresenter(buildLayoutService, _view);
            _presenter.SetupView(new LayoutRuleDataRepository());
        }

        [MenuItem("Window/Smart Addresser/Layout Viewer")]
        public static void Open()
        {
            GetWindow<LayoutViewerWindow>(WindowName);
        }
    }
}
