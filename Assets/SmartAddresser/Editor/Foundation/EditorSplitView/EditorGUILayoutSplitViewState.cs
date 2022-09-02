using System;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.EditorSplitView
{
    [Serializable]
    public sealed class EditorGUILayoutSplitViewState
    {
        [SerializeField] private LayoutDirection _direction;
        [SerializeField] private float _firstRectMinSize;
        [SerializeField] private float _secondRectMinSize;
        [SerializeField] private float _normalizedPosition;

        public EditorGUILayoutSplitViewState(LayoutDirection direction, float initialNormalizedPosition = 0.5f,
            float firstRectMinSize = 1.0f, float secondRectMinSize = 1.0f)
        {
            _direction = direction;
            _firstRectMinSize = firstRectMinSize;
            _secondRectMinSize = secondRectMinSize;
            _normalizedPosition = initialNormalizedPosition;
        }

        public LayoutDirection Direction
        {
            get => _direction;
            internal set => _direction = value;
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

        public float NormalizedPosition
        {
            get => _normalizedPosition;
            internal set => _normalizedPosition = value;
        }
    }
}
