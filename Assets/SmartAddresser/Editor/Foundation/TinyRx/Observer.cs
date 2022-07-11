// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Runtime.ExceptionServices;

namespace SmartAddresser.Editor.Foundation.TinyRx
{
    /// <summary>
    ///     Implementation of <see cref="IObserver{T}" /> that can be used anonymously.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Observer<T> : IObserver<T>
    {
        private readonly Action _onCompleted;
        private readonly Action<Exception> _onError;
        private readonly Action<T> _onNext;

        public Observer(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null)
        {
            _onNext = onNext;
            _onError = onError;
            _onCompleted = onCompleted;
        }

        public void OnNext(T value)
        {
            _onNext?.Invoke(value);
        }

        public void OnError(Exception error)
        {
            if (_onError != null)
            {
                _onError?.Invoke(error);
            }
            else
            {
                ExceptionDispatchInfo.Capture(error).Throw();
            }
        }

        public void OnCompleted()
        {
            _onCompleted?.Invoke();
        }
    }
}
