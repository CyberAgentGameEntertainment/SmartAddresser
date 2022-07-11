// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEditor;

namespace SmartAddresser.Editor.Foundation.ListableProperty
{
    /// <summary>
    ///     <para>Non-generic <see cref="DefaultAsset" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class DefaultAssetListableProperty : ListableProperty<DefaultAsset>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="UnityEditor.SceneAsset" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class SceneAssetListableProperty : ListableProperty<SceneAsset>
    {
    }

    /// <summary>
    ///     <para>Non-generic <see cref="UnityEditor.MonoScript" /> type implementation of <see cref="ListableProperty{T}" />.</para>
    ///     <para>
    ///         If you use a version of Unity older than 2019, Unity cannot serialize generic type.
    ///         So you need to use this non-generic class.
    ///     </para>
    /// </summary>
    [Serializable]
    public sealed class MonoScriptListableProperty : ListableProperty<MonoScript>
    {
    }
}
