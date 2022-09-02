using System;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.EditorSplitView
{
    [Serializable]
    public sealed class EditorGUISplitViewState
    {
        [SerializeField] private float _firstRectSize;
        [SerializeField] private LayoutDirection _layoutDirection;
        [SerializeField] private float _firstRectMinSize;
        [SerializeField] private float _secondRectMinSize;

        public EditorGUISplitViewState(LayoutDirection layoutDirection, float initialSize,
            float firstRectMinSize = 0.0f, float secondRectMinSize = 0.0f)
        {
            _layoutDirection = layoutDirection;
            _firstRectSize = initialSize;
            _firstRectMinSize = firstRectMinSize;
            _secondRectMinSize = secondRectMinSize;
        }

        public float FirstRectSize
        {
            get => _firstRectSize;
            internal set => _firstRectSize = value;
        }

        public LayoutDirection LayoutDirection
        {
            get => _layoutDirection;
            internal set => _layoutDirection = value;
        }

        public float FirstRectMinSize
        {
            get => _firstRectMinSize;
            internal set => _firstRectMinSize = value;
        }

        public float SecondRectMinSize
        {
            get => _secondRectMinSize;
            internal set => _secondRectMinSize = value;
        }
    }
}
