// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty
{
    [Serializable]
    public class Vector2ObservableProperty : ObservableProperty<Vector2>
    {
        public Vector2ObservableProperty()
        {
        }

        public Vector2ObservableProperty(Vector2 value) : base(value)
        {
        }
    }

    [Serializable]
    public class Vector3ObservableProperty : ObservableProperty<Vector3>
    {
        public Vector3ObservableProperty()
        {
        }

        public Vector3ObservableProperty(Vector3 value) : base(value)
        {
        }
    }

    [Serializable]
    public class Vector4ObservableProperty : ObservableProperty<Vector4>
    {
        public Vector4ObservableProperty()
        {
        }

        public Vector4ObservableProperty(Vector4 value) : base(value)
        {
        }
    }

    [Serializable]
    public class RectObservableProperty : ObservableProperty<Rect>
    {
        public RectObservableProperty()
        {
        }

        public RectObservableProperty(Rect value) : base(value)
        {
        }
    }

    [Serializable]
    public class QuaternionObservableProperty : ObservableProperty<Quaternion>
    {
        public QuaternionObservableProperty()
        {
        }

        public QuaternionObservableProperty(Quaternion value) : base(value)
        {
        }
    }

    [Serializable]
    public class Matrix4x4ObservableProperty : ObservableProperty<Matrix4x4>
    {
        public Matrix4x4ObservableProperty()
        {
        }

        public Matrix4x4ObservableProperty(Matrix4x4 value) : base(value)
        {
        }
    }

    [Serializable]
    public class ColorObservableProperty : ObservableProperty<Color>
    {
        public ColorObservableProperty()
        {
        }

        public ColorObservableProperty(Color value) : base(value)
        {
        }
    }

    [Serializable]
    public class Color32ObservableProperty : ObservableProperty<Color32>
    {
        public Color32ObservableProperty()
        {
        }

        public Color32ObservableProperty(Color32 value) : base(value)
        {
        }
    }
}
