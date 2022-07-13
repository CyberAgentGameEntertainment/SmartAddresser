using System;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.EditorSplitView
{
    [Serializable]
    public sealed class EditorGUILayoutSplitView
    {
        [SerializeField] private LayoutDirection _direction;
        [SerializeField] private float _firstRectMinSize;
        [SerializeField] private float _secondRectMinSize;
        [SerializeField] private float _normalizedPosition;
        private bool _isResizing;
        private float _maxPosition;
        private Vector2 _scrollPosition1;
        private Vector2 _scrollPosition2;

        public float NormalizedPosition => _normalizedPosition;

        public EditorGUILayoutSplitView(LayoutDirection direction, float initialNormalizedPosition = 0.5f,
            float firstRectMinSize = 1.0f, float secondRectMinSize = 1.0f)
        {
            _direction = direction;
            _firstRectMinSize = Mathf.Max(1.0f, firstRectMinSize);
            _secondRectMinSize = Mathf.Max(1.0f, secondRectMinSize);
            _normalizedPosition = initialNormalizedPosition;
        }

        public void Begin()
        {
            var isHorizontal = _direction == LayoutDirection.Horizontal;

            if (isHorizontal)
            {
                var rect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                if (Event.current.type == EventType.Repaint)
                    _maxPosition = rect.width;
            }
            else
            {
                var rect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                if (Event.current.type == EventType.Repaint)
                    _maxPosition = rect.height;
            }

            if (isHorizontal)
                _scrollPosition1 = GUILayout.BeginScrollView(_scrollPosition1,
                    GUILayout.Width(_normalizedPosition * _maxPosition));
            else
                _scrollPosition1 = GUILayout.BeginScrollView(_scrollPosition1,
                    GUILayout.Height(_normalizedPosition * _maxPosition));
        }

        /// <summary>
        /// </summary>
        /// <returns>Return true if resized.</returns>
        public bool Split()
        {
            var isHorizontal = _direction == LayoutDirection.Horizontal;

            GUILayout.EndScrollView();

            // Draw border.
            var borderRectOptions = isHorizontal ? GUILayout.ExpandHeight(true) : GUILayout.ExpandWidth(true);
            var borderRect = GUILayoutUtility.GetRect(1, 1, borderRectOptions);
            EditorGUI.DrawRect(borderRect, SplitViewUtility.EditorBorderColor);

            // Draw cursor.
            var cursorRect = borderRect;
            if (isHorizontal)
            {
                cursorRect.xMin -= 5;
                cursorRect.xMax += 5;
            }
            else
            {
                cursorRect.yMin -= 5;
                cursorRect.yMax += 5;
            }

            EditorGUIUtility.AddCursorRect(cursorRect,
                isHorizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);

            // Observe mouse events.
            if (Event.current.type == EventType.MouseDown && cursorRect.Contains(Event.current.mousePosition))
                _isResizing = true;

            if (_isResizing)
            {
                if (_direction == LayoutDirection.Horizontal)
                {
                    _normalizedPosition = Event.current.mousePosition.x;
                    _normalizedPosition = Mathf.Clamp(_normalizedPosition, _firstRectMinSize,
                        _maxPosition - _firstRectMinSize);
                    _normalizedPosition /= _maxPosition;
                }
                else
                {
                    _normalizedPosition = Event.current.mousePosition.y;
                    _normalizedPosition = Mathf.Clamp(_normalizedPosition, _secondRectMinSize,
                        _maxPosition - _secondRectMinSize);
                    _normalizedPosition /= _maxPosition;
                }
            }

            if (Event.current.type == EventType.MouseUp)
                _isResizing = false;

            // Start ScrollView.
            if (isHorizontal)
                _scrollPosition2 = GUILayout.BeginScrollView(_scrollPosition2,
                    GUILayout.Width((1 - _normalizedPosition) * _maxPosition));
            else
                _scrollPosition2 = GUILayout.BeginScrollView(_scrollPosition2,
                    GUILayout.Height((1 - _normalizedPosition) * _maxPosition));

            return _isResizing;
        }

        public void End()
        {
            GUILayout.EndScrollView();

            if (_direction == LayoutDirection.Horizontal)
                EditorGUILayout.EndHorizontal();
            else
                EditorGUILayout.EndVertical();
        }
    }
}
