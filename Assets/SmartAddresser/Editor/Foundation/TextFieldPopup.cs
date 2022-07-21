using System;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation
{
    internal sealed class TextFieldPopup : PopupWindowContent
    {
        private const float Padding = 8.0f;

        private readonly Action<string> _changed;
        private readonly Action<string> _closed;
        private readonly string _message;
        private readonly GUIStyle _messageLabelStyle;
        private readonly Vector2 _windowSize;

        private bool _hasFirstOnGUICalled;

        private string _text;

        private TextFieldPopup(string text, Action<string> changed, Action<string> closed, string message = null,
            float width = 300)
        {
            _message = message;
            _text = text;
            _changed = changed;
            _closed = closed;

            _messageLabelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                wordWrap = true
            };

            var labelWidth = width - Padding * 2;
            _windowSize = Vector2.zero;
            _windowSize.x = width;
            _windowSize.y += Padding; // Space
            _windowSize.y += _messageLabelStyle.CalcHeight(new GUIContent(message), labelWidth); // Message
            _windowSize.y += EditorGUIUtility.standardVerticalSpacing; // Space
            _windowSize.y += EditorGUIUtility.singleLineHeight; // TextField
            _windowSize.y += Padding; // Space
        }

        public static void Show(Vector2 position, string text, Action<string> changed, Action<string> closed,
            string message = null, float width = 300)
        {
            var rect = new Rect(position, Vector2.zero);
            var content = new TextFieldPopup(text, changed, closed, message, width);
            PopupWindow.Show(rect, content);
        }

        public override void OnGUI(Rect rect)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
                editorWindow.Close();

            var textFieldName = $"{GetType().Name}{nameof(_text)}";
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(Padding);
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField(_message, _messageLabelStyle);
                    using (var ccs = new EditorGUI.ChangeCheckScope())
                    {
                        GUI.SetNextControlName(textFieldName);
                        _text = EditorGUILayout.TextField(_text);
                        if (ccs.changed)
                            _changed?.Invoke(_text);
                    }
                }

                GUILayout.Space(Padding);
            }

            if (!_hasFirstOnGUICalled)
            {
                GUI.FocusControl(textFieldName);
                _hasFirstOnGUICalled = true;
            }
        }

        public override void OnClose()
        {
            _closed?.Invoke(_text);
            base.OnClose();
        }

        public override Vector2 GetWindowSize()
        {
            return _windowSize;
        }
    }
}
