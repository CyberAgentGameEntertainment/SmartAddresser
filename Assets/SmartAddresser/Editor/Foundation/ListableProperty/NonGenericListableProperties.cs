// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SmartAddresser.Editor.Foundation.ListableProperty
{
    /// <summary>
    ///     <para>Non-generic <see cref="int" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class IntListableProperty : ListableProperty<int>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="long" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class LongListableProperty : ListableProperty<long>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="byte" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class ByteListableProperty : ListableProperty<byte>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="float" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class FloatListableProperty : ListableProperty<float>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="double" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class DoubleListableProperty : ListableProperty<double>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="string" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class StringListableProperty : ListableProperty<string>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="bool" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class BoolListableProperty : ListableProperty<bool>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="Vector2" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class Vector2ListableProperty : ListableProperty<Vector2>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="Vector3" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class Vector3ListableProperty : ListableProperty<Vector3>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="Vector4" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class Vector4ListableProperty : ListableProperty<Vector4>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="Color" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class ColorListableProperty : ListableProperty<Color>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="Rect" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class RectListableProperty : ListableProperty<Rect>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="AnimationCurve" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class AnimationCurveListableProperty : ListableProperty<AnimationCurve>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="Bounds" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class BoundsListableProperty : ListableProperty<Bounds>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="Quaternion" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class QuaternionListableProperty : ListableProperty<Quaternion>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="UnityEngine.Object" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class ObjectListableProperty : ListableProperty<Object>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="UnityEngine.GameObject" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class GameObjectListableProperty : ListableProperty<GameObject>
    {
    }

    /// <summary>
    ///     <para>
    ///         Non-generic <see cref="UnityEngine.MonoBehaviour" /> type implementation of
    ///         <see cref="ListableProperty{T}" />.
    ///     </para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class MonoBehaviourListableProperty : ListableProperty<MonoBehaviour>
    {
    }
}
