// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty
{
    /// <summary>
    ///     A class that wraps the <see cref="IObservableProperty{T}" /> and makes it read-only.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReadOnlyObservableProperty<T> : IObservable<T>
    {
        private readonly IObservableProperty<T> _source;
        private bool _didDispose;

        public ReadOnlyObservableProperty(IObservableProperty<T> source)
        {
            _source = source;
        }

        /// <summary>
        ///     Current Value.
        /// </summary>
        public T Value => _source.Value;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_didDispose)
            {
                throw new ObjectDisposedException(nameof(ReadOnlyObservableProperty<T>));
            }

            return _source.Subscribe(observer);
        }

        public void Dispose()
        {
            if (_didDispose)
            {
                throw new ObjectDisposedException(nameof(ReadOnlyObservableProperty<T>));
            }

            _source.Dispose();
            _didDispose = true;
        }
    }
}
