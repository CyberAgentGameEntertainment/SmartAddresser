using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection
{
    public abstract class ObservableListBase<T> : IObservableList<T>, IReadOnlyObservableList<T>, IDisposable
    {
        private readonly Subject<ListAddEvent<T>> _subjectAdd = new Subject<ListAddEvent<T>>();
        private readonly Subject<Empty> _subjectClear = new Subject<Empty>();
        private readonly Subject<ListRemoveEvent<T>> _subjectRemove = new Subject<ListRemoveEvent<T>>();
        private readonly Subject<ListReplaceEvent<T>> _subjectReplace = new Subject<ListReplaceEvent<T>>();

        private bool _didDispose;

        protected ObservableListBase()
        {
        }

        protected ObservableListBase(IEnumerable<T> items)
        {
            InternalList.AddRange(items);
        }

        protected abstract List<T> InternalList { get; }

        public void Dispose()
        {
            DisposeSubject(_subjectAdd);
            DisposeSubject(_subjectRemove);
            DisposeSubject(_subjectClear);
            DisposeSubject(_subjectReplace);

            _didDispose = true;
        }

        public IObservable<ListAddEvent<T>> ObservableAdd => _subjectAdd;

        public IObservable<ListRemoveEvent<T>> ObservableRemove => _subjectRemove;

        public IObservable<Empty> ObservableClear => _subjectClear;

        public IObservable<ListReplaceEvent<T>> ObservableReplace => _subjectReplace;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => InternalList[index];
            set
            {
                var oldValue = InternalList[index];
                if (Equals(oldValue, value)) return;

                InternalList[index] = value;
                _subjectReplace.OnNext(new ListReplaceEvent<T>(index, oldValue, value));
            }
        }

        public void Add(T item)
        {
            Assert.IsFalse(_didDispose);
            Assert.IsFalse(item == null);

            InternalList.Add(item);
            var index = InternalList.Count - 1;
            _subjectAdd.OnNext(new ListAddEvent<T>(index, item));
        }

        public void Insert(int index, T item)
        {
            Assert.IsFalse(_didDispose);
            Assert.IsTrue(index >= 0);
            Assert.IsFalse(item == null);

            InternalList.Insert(index, item);
            _subjectAdd.OnNext(new ListAddEvent<T>(index, item));
        }

        public bool Remove(T item)
        {
            Assert.IsFalse(_didDispose);
            Assert.IsFalse(item == null);

            var index = InternalList.IndexOf(item);
            if (index < 0) return false;

            InternalList.RemoveAt(index);
            _subjectRemove.OnNext(new ListRemoveEvent<T>(index, item));
            return true;
        }

        public void RemoveAt(int index)
        {
            Assert.IsFalse(_didDispose);
            Assert.IsTrue(index >= 0);

            var item = InternalList[index];
            InternalList.RemoveAt(index);
            _subjectRemove.OnNext(new ListRemoveEvent<T>(index, item));
        }

        public void Clear()
        {
            Assert.IsFalse(_didDispose);

            InternalList.Clear();
            _subjectClear.OnNext(Empty.Default);
        }

        public int Count => InternalList.Count;

        public bool Contains(T item)
        {
            Assert.IsFalse(_didDispose);

            return InternalList.Contains(item);
        }

        public int IndexOf(T item)
        {
            Assert.IsFalse(_didDispose);

            return InternalList.IndexOf(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Assert.IsFalse(_didDispose);
            Assert.IsNotNull(array);

            InternalList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        private static void DisposeSubject<TSubjectValue>(Subject<TSubjectValue> subject)
        {
            if (subject.DidDispose) return;

            if (subject.DidTerminate)
            {
                subject.Dispose();
                return;
            }

            subject.OnCompleted();
            subject.Dispose();
        }
    }
}
