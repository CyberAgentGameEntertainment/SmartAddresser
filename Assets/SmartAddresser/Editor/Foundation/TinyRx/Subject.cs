using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace SmartAddresser.Editor.Foundation.TinyRx
{
    /// <summary>
    ///     Subject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Subject<T> : IObserver<T>, IObservable<T>
    {
        private readonly HashSet<IObserver<T>> _observers = new HashSet<IObserver<T>>();

        /// <summary>
        ///     If <see cref="Dispose" /> is already called, return true.
        /// </summary>
        public bool DidDispose { get; private set; }

        /// <summary>
        ///     If <see cref="OnCompleted" /> or <see cref="OnError" /> is already called, return true.
        /// </summary>
        public bool DidTerminate { get; private set; }

        public Exception Error { get; private set; }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Assert.IsNotNull(observer);
            Assert.IsFalse(DidDispose);

            // If already has been terminated.
            if (DidTerminate)
            {
                if (Error != null)
                    observer.OnError(Error);
                else
                    observer.OnCompleted();
            }

            _observers.Add(observer);
            return new Disposable(() => OnObserverDispose(observer));
        }

        public void OnNext(T value)
        {
            Assert.IsFalse(DidDispose);
            Assert.IsFalse(DidTerminate);

            var observers = _observers.ToArray();
            foreach (var observer in observers)
            {
                if (DidDispose)
                    break;

                if (DidTerminate)
                    break;

                if (!_observers.Contains(observer))
                    continue;

                observer.OnNext(value);
            }
        }

        public void OnError(Exception error)
        {
            Assert.IsNotNull(error);
            Assert.IsFalse(DidDispose);
            Assert.IsFalse(DidTerminate);

            var observers = _observers.ToArray();
            foreach (var observer in observers)
            {
                if (DidDispose)
                    break;

                if (DidTerminate)
                    break;

                if (!_observers.Contains(observer))
                    continue;

                observer.OnError(error);
            }

            DidTerminate = true;
            Error = error;
        }

        public void OnCompleted()
        {
            Assert.IsFalse(DidDispose);
            Assert.IsFalse(DidTerminate);

            var observers = _observers.ToArray();
            foreach (var observer in observers)
            {
                if (DidDispose)
                    break;

                if (DidTerminate)
                    break;

                if (!_observers.Contains(observer))
                    continue;

                observer.OnCompleted();
            }

            DidTerminate = true;
        }

        public void Dispose()
        {
            _observers.Clear();
            DidDispose = true;
        }

        private void OnObserverDispose(IObserver<T> value)
        {
            _observers.Remove(value);
        }
    }
}
