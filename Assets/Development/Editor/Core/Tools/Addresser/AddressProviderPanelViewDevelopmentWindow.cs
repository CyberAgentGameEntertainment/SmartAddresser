using SmartAddresser.Editor.Core.Models.EntryRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace Development.Editor.Core.Tools.Addresser
{
    internal sealed class AddressProviderPanelViewDevelopmentWindow : EditorWindow
    {
        private const string WindowName = "[Dev] Address Provider Panel View";
        private AutoIncrementHistory _history;
        private AddressProviderPanelViewPresenter _presenter;
        private AddressProviderPanelView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);

            var providerProperty = new ObservableProperty<IAddressProvider>(new FakeAddressProvider());
            _view = new AddressProviderPanelView(providerProperty);
            _history = new AutoIncrementHistory();
            _presenter =
                new AddressProviderPanelViewPresenter(providerProperty, _view, _history, new FakeAssetSaveService());
        }

        private void OnDisable()
        {
            _presenter.Dispose();
            _view.Dispose();
        }

        private void OnGUI()
        {
            var e = Event.current;
            if (GetEventAction(e) && e.type == EventType.KeyDown && e.keyCode == KeyCode.Z)
            {
                _history.Undo();
                e.Use();
            }

            if (GetEventAction(e) && e.type == EventType.KeyDown && e.keyCode == KeyCode.Y)
            {
                _history.Redo();
                e.Use();
            }

            _view.DoLayout();
        }

        private bool GetEventAction(Event e)
        {
#if UNITY_EDITOR_WIN
            return e.control;
#else
            return e.command;
#endif
        }

        [MenuItem("Window/Smart Addresser/Development/Address Provider Panel View")]
        public static void Open()
        {
            GetWindow<AddressProviderPanelViewDevelopmentWindow>(WindowName);
        }
    }
}
