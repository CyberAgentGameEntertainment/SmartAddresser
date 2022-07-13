using UnityEngine;

namespace SmartAddresser.Editor.Foundation.StateBasedUndo
{
    /// <summary>
    ///     Take and restore a snapshot of an object's state by using Unity's JsonUtility.
    /// </summary>
    public sealed class UnityJsonObjectStateSnapshot : IObjectStateSnapshot
    {
        /// <summary>
        ///     Initialize.
        /// </summary>
        /// <param name="target"></param>
        public UnityJsonObjectStateSnapshot(object target)
        {
            Target = target;
        }

        /// <summary> Target object. </summary>
        public object Target { get; }

        /// <summary> Serialized data. </summary>
        public string Data { get; set; }

        public void Take()
        {
            Data = JsonUtility.ToJson(Target);
        }

        public void Restore()
        {
            JsonUtility.FromJsonOverwrite(Data, Target);
        }

        public override bool Equals(object obj)
        {
            if (obj is UnityJsonObjectStateSnapshot snapshot)
                return Equals(snapshot);

            return false;
        }

        private bool Equals(UnityJsonObjectStateSnapshot other)
        {
            return Equals(Target, other.Target) && Data == other.Data;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Target != null ? Target.GetHashCode() : 0) * 397) ^ (Data != null ? Data.GetHashCode() : 0);
            }
        }
    }
}
