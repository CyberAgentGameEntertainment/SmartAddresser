using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.OrderCollection
{
    /// <summary>
    ///     Class to manage the order.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public class OrderCollection<TId> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<TId> _ids = new List<TId>();

        private Dictionary<TId, int> _idToIndexMap = new Dictionary<TId, int>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            RebuildIdToIndexMap();
        }

        public int GetCount()
        {
            return _ids.Count;
        }

        public void Add(TId id)
        {
            _ids.Add(id);
            RebuildIdToIndexMap();
        }

        public void Remove(TId id)
        {
            _ids.Remove(id);
            RebuildIdToIndexMap();
        }

        public void Clear()
        {
            _ids.Clear();
            RebuildIdToIndexMap();
        }

        public bool HasIndex(TId id)
        {
            return _idToIndexMap.ContainsKey(id);
        }

        public int GetIndex(TId id)
        {
            return _idToIndexMap[id];
        }

        public bool TryGetIndex(TId id, out int index)
        {
            return _idToIndexMap.TryGetValue(id, out index);
        }

        public void SetIndex(TId id, int index)
        {
            var beforeIndex = _idToIndexMap[id];
            _ids.RemoveAt(beforeIndex);
            _ids.Insert(index, id);
            RebuildIdToIndexMap();
        }

        public TId GetElementAt(int index)
        {
            return _ids[index];
        }

        private void RebuildIdToIndexMap()
        {
            _idToIndexMap.Clear();
            for (var i = 0; i < _ids.Count; i++) _idToIndexMap[_ids[i]] = i;
        }
    }
}
