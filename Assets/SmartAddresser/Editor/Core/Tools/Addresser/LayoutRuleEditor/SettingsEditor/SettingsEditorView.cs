using System;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.SettingsEditor
{
    /// <summary>
    ///     View for settings editor.
    /// </summary>
    internal sealed class SettingsEditorView : IDisposable
    {
        private readonly ObservableProperty<string> _versionExpression = new ObservableProperty<string>();
        private readonly ObservableProperty<bool> _excludeUnversioned = new ObservableProperty<bool>();

        private ICustomDrawer _drawer;

        public IObservableProperty<string> VersionExpression => _versionExpression;
        
        public IObservableProperty<bool> ExcludeUnversioned => _excludeUnversioned;

        public bool Enabled { get; set; }

        public void Dispose()
        {
            _versionExpression.Dispose();
            _excludeUnversioned.Dispose();
        }

        public void DoLayout()
        {
            var enabled = GUI.enabled;
            GUI.enabled = GUI.enabled && Enabled;

            _versionExpression.Value = EditorGUILayout.TextField("Version Expression", _versionExpression.Value);
            _excludeUnversioned.Value = EditorGUILayout.Toggle("Exclude Unversioned", _excludeUnversioned.Value);

            GUI.enabled = enabled;
        }
    }
}
