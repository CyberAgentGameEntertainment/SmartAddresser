// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace SmartAddresser.Editor.Foundation.TinyRx
{
    internal static class ObservableImplExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> self, Action<T> onNext,
            Action<Exception> onError = null, Action onCompleted = null)
        {
            return self.Subscribe(new Observer<T>(onNext, onError, onCompleted));
        }

        /// <summary>
        ///     Suppress the first n items emitted by the Observable.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="count"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObservable<T> Skip<T>(this IObservable<T> self, int count)
        {
            return new AnonymousObservable<T>(observer =>
            {
                var currentCount = 0;
                var disposables = new CompositeDisposable();
                var disposable = self.Subscribe(x =>
                {
                    if (currentCount >= count)
                    {
                        observer.OnNext(x);
                    }

                    currentCount++;
                }, observer.OnError, observer.OnCompleted);
                disposables.Add(disposable);
                return disposables;
            });
        }

        /// <summary>
        ///     Pair the previous item and the current item emitted by the Observable.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="initialValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObservable<(T oldValue, T newValue)> PairWithPrevious<T>(this IObservable<T> self,
            T initialValue)
        {
            return new AnonymousObservable<(T oldValue, T newValue)>(observer =>
            {
                var oldValue = initialValue;
                var disposables = new CompositeDisposable();
                var disposable = self.Subscribe(x =>
                    {
                        observer.OnNext((oldValue, x));
                        oldValue = x;
                    }, observer.OnError,
                    observer.OnCompleted);
                disposables.Add(disposable);
                return disposables;
            });
        }
    }
}
