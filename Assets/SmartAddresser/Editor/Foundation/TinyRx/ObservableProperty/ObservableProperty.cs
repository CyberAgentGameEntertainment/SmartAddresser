using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty
{
    /// <summary>
    ///     Property that can be observed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ObservableProperty<T> : IObservableProperty<T>, IReadOnlyObservableProperty<T>
    {
        [SerializeField] private T _value;
        private readonly HashSet<IObserver<T>> _observers = new HashSet<IObserver<T>>();
        private bool _didDispose;

        public ObservableProperty()
        {
        }

        public ObservableProperty(T initialValueOrReference)
        {
            _value = initialValueOrReference;
        }

        public T Value
        {
            get => _value;
            set => SetValue(value);
        }

        public void Dispose()
        {
            // Clean all observers.
            foreach (var observer in _observers) observer.OnCompleted();
            _observers.Clear();
            _didDispose = true;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Assert.IsNotNull(observer);

            if (_didDispose)
            {
                observer.OnCompleted();
                return new Disposable(null);
            }

            observer.OnNext(Value);
            _observers.Add(observer);
            return new Disposable(() => OnObserverDispose(observer));
        }

        public void SetValueAndNotify(T value)
        {
            SetValue(value, true);
        }

        public void SetValueAndNotNotify(T value)
        {
            Assert.IsFalse(_didDispose);

            _value = value;
        }

        private void OnObserverDispose(IObserver<T> value)
        {
            if (_observers.Remove(value)) value.OnCompleted();
        }

        private void Notify(T value)
        {
            Assert.IsFalse(_didDispose);

            foreach (var observer in _observers) observer.OnNext(value);
        }

        private void SetValue(T value, bool forceNotify = false)
        {
            Assert.IsFalse(_didDispose);

            if (!forceNotify && EqualsInternal(Value, value))
                return;

            _value = value;
            Notify(value);
        }

        /// <summary>
        ///     Determine the equivalence of a and b.
        /// </summary>
        /// <remarks>
        ///     If <see cref="T" /> is the type of a struct that is not implemented <see cref="IEquatable{T}" />,
        ///     you should implement the interface or override this method to prevent memory allocations.
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool EqualsInternal(T a, T b)
        {
            return EqualityComparer<T>.Default.Equals(a, b);
        }
    }
}
