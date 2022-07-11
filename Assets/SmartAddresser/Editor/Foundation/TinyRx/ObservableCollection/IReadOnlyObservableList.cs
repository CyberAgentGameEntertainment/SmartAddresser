// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection
{
    public interface IReadOnlyObservableList<TItem> : IReadOnlyObservableList<TItem, Empty>
    {
    }

    /// <summary>
    ///     The read-only list that can be observed changes of the items.
    /// </summary>
    /// <typeparam name="TItem">Type of items.</typeparam>
    /// <typeparam nameL@="TEmpty"></typeparam>
    public interface IReadOnlyObservableList<TItem, TEmpty> : IReadOnlyList<TItem>
    {
        /// <summary>
        ///     The observable that is called when a item was added.
        /// </summary>
        IObservable<ListAddEvent<TItem>> ObservableAdd { get; }

        /// <summary>
        ///     The observable that is called when a item was removed.
        /// </summary>
        IObservable<ListRemoveEvent<TItem>> ObservableRemove { get; }

        /// <summary>
        ///     The observable that is called when items was cleared.
        /// </summary>
        IObservable<TEmpty> ObservableClear { get; }

        /// <summary>
        ///     The observable that is called when a item was replaced.
        /// </summary>
        IObservable<ListReplaceEvent<TItem>> ObservableReplace { get; }
    }
}
