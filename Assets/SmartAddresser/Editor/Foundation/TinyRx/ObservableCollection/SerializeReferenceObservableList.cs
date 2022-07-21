using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection
{
    [Serializable]
    public class SerializeReferenceObservableList<T> : ObservableListBase<T>
    {
        [SerializeReference] private List<T> _internalList = new List<T>();

        protected override List<T> InternalList => _internalList;
    }
}
