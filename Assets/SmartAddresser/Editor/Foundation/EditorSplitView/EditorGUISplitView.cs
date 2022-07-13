using System;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.EditorSplitView
{
    [Serializable]
    public sealed class EditorGUISplitView
    {
        private const float ResizeAreaSize = 16.0f;

        [SerializeField] private float _firstRectSize;
        [SerializeField] private LayoutDirection _layoutDirection;
        [SerializeField] private float _firstRectMinSize;
        [SerializeField] private float _secondRectMinSize;
        [NonSerialized] private bool _isResizing;
        [NonSerialized] private float _mousePosDiff;

        public EditorGUISplitView(LayoutDirection layoutDirection, float initialSize, float firstRectMinSize = 0,
            float secondRectMinSize = 0)
        {
            _firstRectSize = initialSize;
            _layoutDirection = layoutDirection;
            _firstRectMinSize = firstRectMinSize;
            _secondRectMinSize = secondRectMinSize;
        }

        /// <summary>
        ///     Draw GUI.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="drawFirstRect"></param>
        /// <param name="drawSecondRect"></param>
        /// <returns>Return true if resized.</returns>
        public bool DrawGUI(Rect rect, Action<Rect> drawFirstRect, Action<Rect> drawSecondRect)
        {
            var isHorizontal = _layoutDirection == LayoutDirection.Horizontal;

            // Resize to mouse position when dragging.
            if (_isResizing && Event.current.type == EventType.MouseDrag)
            {
                var minSize = isHorizontal ? rect.x + _firstRectMinSize : rect.y + _firstRectMinSize;
                var maxSize = isHorizontal
                    ? rect.x + rect.width - _secondRectMinSize
                    : rect.y + rect.height - _secondRectMinSize;
                if (maxSize < minSize) maxSize = minSize;

                var mousePos = isHorizontal ? Event.current.mousePosition.x : Event.current.mousePosition.y;
                _firstRectSize = Mathf.Clamp(mousePos + _mousePosDiff, minSize, maxSize);
                _firstRectSize -= isHorizontal ? rect.x : rect.y;
            }

            // Clamp when the window is resized.
            if (Event.current.type == EventType.Layout)
            {
                var minSize = isHorizontal ? rect.x + _firstRectMinSize : rect.y + _firstRectMinSize;
                var maxSize = isHorizontal
                    ? rect.x + rect.width - _secondRectMinSize
                    : rect.y + rect.height - _secondRectMinSize;
                if (maxSize < minSize) maxSize = minSize;

                _firstRectSize = Mathf.Clamp(_firstRectSize, minSize, maxSize);
                _firstRectSize -= isHorizontal ? rect.x : rect.y;
            }

            // Calculate each rectanble.
            var firstRect = rect;
            firstRect.width = isHorizontal ? _firstRectSize : firstRect.width;
            firstRect.height = isHorizontal ? firstRect.height : _firstRectSize;

            var borderRect = rect;
            borderRect.x += isHorizontal ? firstRect.width : 0;
            borderRect.y += isHorizontal ? 0 : firstRect.height;
            borderRect.width = isHorizontal ? 1 : borderRect.width;
            borderRect.height = isHorizontal ? borderRect.height : 1;

            var secondRect = rect;
            secondRect.x += isHorizontal ? firstRect.width + 1 : 0;
            secondRect.y += isHorizontal ? 0 : firstRect.height + 1;
            secondRect.height = isHorizontal ? firstRect.height : rect.height - _firstRectSize - 1;
            secondRect.width = isHorizontal ? rect.width - _firstRectSize - 1 : firstRect.width;

            var resizeAreaRect = borderRect;
            resizeAreaRect.width = isHorizontal ? ResizeAreaSize : resizeAreaRect.width;
            resizeAreaRect.height = isHorizontal ? resizeAreaRect.height : ResizeAreaSize;
            resizeAreaRect.x -= isHorizontal ? ResizeAreaSize / 2.0f : 1;
            resizeAreaRect.y -= isHorizontal ? 0 : ResizeAreaSize / 2.0f;

            // Handle mouse up/down events.
            if (Event.current.type == EventType.MouseDown && resizeAreaRect.Contains(Event.current.mousePosition))
            {
                var mousePos = isHorizontal ? Event.current.mousePosition.x : Event.current.mousePosition.y;
                var borderPos = isHorizontal ? borderRect.x : borderRect.y;
                _mousePosDiff = borderPos - mousePos;
                _isResizing = true;
            }

            if (Event.current.type == EventType.MouseUp) _isResizing = false;

            // Draw each GUI.
            drawFirstRect(firstRect);
            drawSecondRect(secondRect);
            EditorGUIUtility.AddCursorRect(resizeAreaRect,
                isHorizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);
            EditorGUI.DrawRect(borderRect, SplitViewUtility.EditorBorderColor);

            return _isResizing;
        }
    }
}
