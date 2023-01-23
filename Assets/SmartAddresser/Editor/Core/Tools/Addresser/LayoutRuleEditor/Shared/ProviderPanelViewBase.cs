using System;
using SmartAddresser.Editor.Core.Tools.Shared;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using SmartAddresser.Editor.Foundation.TinyRx;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared
{
    /// <summary>
    ///     View for each provider panel.
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    internal abstract class ProviderPanelViewBase<TProvider> : IDisposable where TProvider : class
    {
        private readonly Subject<Empty> _changeProviderButtonClickedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _mouseButtonClickedSubject = new Subject<Empty>();
        private readonly Subject<Empty> _providerValueChangedSubject = new Subject<Empty>();

        private ICustomDrawer _drawer;

        public IObservable<Empty> MouseButtonClickedAsObservable => _mouseButtonClickedSubject;
        public IObservable<Empty> ProviderValueChangedAsObservable => _providerValueChangedSubject;
        public IObservable<Empty> ChangeProviderButtonClickedAsObservable => _changeProviderButtonClickedSubject;

        public TProvider Provider { get; private set; }

        public bool Enabled { get; set; }

        public void Dispose()
        {
            _mouseButtonClickedSubject.Dispose();
            _providerValueChangedSubject.Dispose();
            _changeProviderButtonClickedSubject.Dispose();
        }

        public void DoLayout()
        {
            var enabled = GUI.enabled;
            GUI.enabled = GUI.enabled && Enabled;

            // Click
            if (Event.current.type == EventType.MouseDown)
                _mouseButtonClickedSubject.OnNext(Empty.Default);

            if (_drawer != null)
            {
                // Provider
                using var ccs = new EditorGUI.ChangeCheckScope();
                _drawer.DoLayout();

                // Notify when any value is changed.
                if (ccs.changed)
                    _providerValueChangedSubject.OnNext(Empty.Default);

                // Border
                GUILayout.Space(4);
                var borderRect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
                EditorGUI.DrawRect(borderRect, EditorGUIUtil.EditorBorderColor);
            }

            // Change provider button
            var bottomRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight + 8,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            var buttonRect = bottomRect;
            buttonRect.height = EditorGUIUtility.singleLineHeight;
            buttonRect.y += 4;
            buttonRect.x = buttonRect.width / 2.0f - 60;
            buttonRect.width = 120;
            if (GUI.Button(buttonRect, "Change Provider"))
                _changeProviderButtonClickedSubject.OnNext(Empty.Default);

            GUI.enabled = enabled;
        }

        public void SetProvider(TProvider provider)
        {
            if (provider == null)
            {
                _drawer = null;
                Provider = null;
                return;
            }

            var drawer = CustomDrawerFactory.Create(provider.GetType());
            if (drawer == null)
            {
                Debug.LogError($"Drawer of {provider.GetType().Name} is not found.");
                _drawer = null;
                Provider = null;
                return;
            }

            drawer.Setup(provider);
            _drawer = drawer;
            Provider = provider;
        }
    }
}
