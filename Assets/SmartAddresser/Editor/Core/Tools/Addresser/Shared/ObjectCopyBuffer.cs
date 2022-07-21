using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.Shared
{
    /// <summary>
    ///     Buffer to copy and paste serializable object data.
    /// </summary>
    public static class ObjectCopyBuffer
    {
        /// <summary>
        ///     Serialized data of the registered object.
        /// </summary>
        public static string Json { get; private set; }

        /// <summary>
        ///     Type of the registered object.
        /// </summary>
        public static Type Type { get; private set; }

        /// <summary>
        ///     Register the object data.
        /// </summary>
        /// <param name="target"></param>
        public static void Register(object target)
        {
            Type = target.GetType();
            Json = JsonUtility.ToJson(target);
        }
    }
}
