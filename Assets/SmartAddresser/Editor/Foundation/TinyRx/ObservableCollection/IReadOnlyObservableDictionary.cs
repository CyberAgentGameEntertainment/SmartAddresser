// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection
{
    public interface IReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue, Empty>
    {
    }

    /// <summary>
    ///     The read-only dictionary that can be observed changes of the items.
    /// </summary>
    /// <typeparam name="TKey">Type of keys.</typeparam>
    /// <typeparam name="TValue">Type of items.</typeparam>
    /// <typeparam name="TEmpty"></typeparam>
    public interface IReadOnlyObservableDictionary<TKey, TValue, TEmpty> : IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary>
        ///     The observable that is called when a item was added.
        /// </summary>
        IObservable<DictionaryAddEvent<TKey, TValue>> ObservableAdd { get; }

        /// <summary>
        ///     The observable that is called when a item was removed.
        /// </summary>
        IObservable<DictionaryRemoveEvent<TKey, TValue>> ObservableRemove { get; }

        /// <summary>
        ///     The observable that is called when items was cleared.
        /// </summary>
        IObservable<TEmpty> ObservableClear { get; }

        /// <summary>
        ///     The observable that is called when a item was replaced.
        /// </summary>
        IObservable<DictionaryReplaceEvent<TKey, TValue>> ObservableReplace { get; }
    }
}
