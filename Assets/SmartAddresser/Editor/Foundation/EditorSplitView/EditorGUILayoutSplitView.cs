using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.EditorSplitView
{
    public sealed class EditorGUILayoutSplitView
    {
        private bool _isResizing;
        private float _maxPosition;
        private Vector2 _scrollPosition1;
        private Vector2 _scrollPosition2;

        public EditorGUILayoutSplitView(EditorGUILayoutSplitViewState state)
        {
            State = state;
        }

        public EditorGUILayoutSplitViewState State { get; }

        public void Begin()
        {
            var isHorizontal = State.Direction == LayoutDirection.Horizontal;

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
                    GUILayout.Width(State.NormalizedPosition * _maxPosition));
            else
                _scrollPosition1 = GUILayout.BeginScrollView(_scrollPosition1,
                    GUILayout.Height(State.NormalizedPosition * _maxPosition));
        }

        /// <summary>
        /// </summary>
        /// <returns>Return true if resized.</returns>
        public bool Split()
        {
            var isHorizontal = State.Direction == LayoutDirection.Horizontal;

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
                if (State.Direction == LayoutDirection.Horizontal)
                {
                    State.NormalizedPosition = Event.current.mousePosition.x;
                    State.NormalizedPosition = Mathf.Clamp(State.NormalizedPosition, State.FirstRectMinSize,
                        _maxPosition - State.FirstRectMinSize);
                    State.NormalizedPosition /= _maxPosition;
                }
                else
                {
                    State.NormalizedPosition = Event.current.mousePosition.y;
                    State.NormalizedPosition = Mathf.Clamp(State.NormalizedPosition, State.SecondRectMinSize,
                        _maxPosition - State.SecondRectMinSize);
                    State.NormalizedPosition /= _maxPosition;
                }
            }

            if (Event.current.type == EventType.MouseUp)
                _isResizing = false;

            // Start ScrollView.
            if (isHorizontal)
                _scrollPosition2 = GUILayout.BeginScrollView(_scrollPosition2,
                    GUILayout.Width((1 - State.NormalizedPosition) * _maxPosition));
            else
                _scrollPosition2 = GUILayout.BeginScrollView(_scrollPosition2,
                    GUILayout.ExpandHeight(true));

            return _isResizing;
        }

        public void End()
        {
            GUILayout.EndScrollView();

            if (State.Direction == LayoutDirection.Horizontal)
                EditorGUILayout.EndHorizontal();
            else
                EditorGUILayout.EndVertical();
        }
    }
}
