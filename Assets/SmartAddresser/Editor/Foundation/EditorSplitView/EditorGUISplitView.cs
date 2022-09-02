using System;
using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.EditorSplitView
{
    public sealed class EditorGUISplitView
    {
        private const float ResizeAreaSize = 16.0f;
        private bool _isResizing;
        private float _mousePosDiff;

        public EditorGUISplitView(EditorGUISplitViewState state)
        {
            State = state;
        }

        public EditorGUISplitViewState State { get; }

        /// <summary>
        ///     Draw GUI.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="drawFirstRect"></param>
        /// <param name="drawSecondRect"></param>
        /// <returns>Return true if resized.</returns>
        public bool DrawGUI(Rect rect, Action<Rect> drawFirstRect, Action<Rect> drawSecondRect)
        {
            var isHorizontal = State.LayoutDirection == LayoutDirection.Horizontal;

            // Resize to mouse position when dragging.
            if (_isResizing && Event.current.type == EventType.MouseDrag)
            {
                var minSize = isHorizontal ? rect.x + State.FirstRectMinSize : rect.y + State.FirstRectMinSize;
                var maxSize = isHorizontal
                    ? rect.x + rect.width - State.SecondRectMinSize
                    : rect.y + rect.height - State.SecondRectMinSize;
                if (maxSize < minSize) maxSize = minSize;

                var mousePos = isHorizontal ? Event.current.mousePosition.x : Event.current.mousePosition.y;
                State.FirstRectSize = Mathf.Clamp(mousePos + _mousePosDiff, minSize, maxSize);
                State.FirstRectSize -= isHorizontal ? rect.x : rect.y;
            }

            // Clamp when the window is resized.
            if (Event.current.type == EventType.Layout)
            {
                var minSize = isHorizontal ? rect.x + State.FirstRectMinSize : rect.y + State.FirstRectMinSize;
                var maxSize = isHorizontal
                    ? rect.x + rect.width - State.SecondRectMinSize
                    : rect.y + rect.height - State.SecondRectMinSize;
                if (maxSize < minSize) maxSize = minSize;

                State.FirstRectSize = Mathf.Clamp(State.FirstRectSize, minSize, maxSize);
                State.FirstRectSize -= isHorizontal ? rect.x : rect.y;
            }

            // Calculate each rectanble.
            var firstRect = rect;
            firstRect.width = isHorizontal ? State.FirstRectSize : firstRect.width;
            firstRect.height = isHorizontal ? firstRect.height : State.FirstRectSize;

            var borderRect = rect;
            borderRect.x += isHorizontal ? firstRect.width : 0;
            borderRect.y += isHorizontal ? 0 : firstRect.height;
            borderRect.width = isHorizontal ? 1 : borderRect.width;
            borderRect.height = isHorizontal ? borderRect.height : 1;

            var secondRect = rect;
            secondRect.x += isHorizontal ? firstRect.width + 1 : 0;
            secondRect.y += isHorizontal ? 0 : firstRect.height + 1;
            secondRect.height = isHorizontal ? firstRect.height : rect.height - State.FirstRectSize - 1;
            secondRect.width = isHorizontal ? rect.width - State.FirstRectSize - 1 : firstRect.width;

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
