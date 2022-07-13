// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Foundation.ListableProperty
{
    /// <summary>
    ///     <para>Property that can be treated as a single value or as an array.</para>
    ///     <para>
    ///         If you serialize this property, it will appear in the Inspector as a non-array property.
    ///         If you want to treat it as array, click the right button of the property to switch it to an array.
    ///     </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ListableProperty<T> : IEnumerable<T>
    {
        [SerializeField] private bool _isListMode;
        [SerializeField] private List<T> _values = new List<T>();

        public ListableProperty(bool isListMode = false)
        {
            _isListMode = isListMode;
        }

        internal List<T> InternalList => _values;

        public T this[int index]
        {
            get
            {
                Assert.IsTrue(_isListMode);
                return _values[index];
            }
            set
            {
                Assert.IsTrue(_isListMode);
                _values[index] = value;
            }
        }

        /// <summary>
        ///     If true, you can use this property as an array.
        /// </summary>
        public bool IsListMode
        {
            get => _isListMode;
            set => _isListMode = value;
        }

        /// <summary>
        ///     Value. You can only use this property not in ListMode.
        /// </summary>
        public T Value
        {
            get
            {
                Assert.IsFalse(_isListMode);
                if (_values.Count == 0) _values.Add(default);

                return _values[0];
            }
            set
            {
                Assert.IsFalse(_isListMode);
                if (_values.Count == 0)
                    _values.Add(value);
                else
                    _values[0] = value;
            }
        }

        /// <summary>
        ///     Count. You can only use this property in ListMode.
        /// </summary>
        public int Count
        {
            get
            {
                Assert.IsTrue(_isListMode);
                return _values.Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_isListMode) return _values.AsEnumerable().GetEnumerator();

            if (_values.Count == 0) _values.Add(default);

            IEnumerator<T> FirstValueEnumerator()
            {
                yield return _values[0];
            }

            return FirstValueEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Add a value. You can only use this method in ListMode.
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(T value)
        {
            Assert.IsTrue(_isListMode);
            _values.Add(value);
        }

        /// <summary>
        ///     Remove a value. You can only use this method in ListMode.
        /// </summary>
        /// <param name="value"></param>
        public void RemoveValue(T value)
        {
            Assert.IsTrue(_isListMode);
            _values.Remove(value);
        }

        /// <summary>
        ///     Remove a value at <see cref="index" />. You can only use this method in ListMode.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveValueAt(int index)
        {
            Assert.IsTrue(_isListMode);
            _values.RemoveAt(index);
        }

        /// <summary>
        ///     Clear all values. You can only use this method in ListMode.
        /// </summary>
        public void ClearValues()
        {
            Assert.IsTrue(_isListMode);
            _values.Clear();
        }
    }
}
