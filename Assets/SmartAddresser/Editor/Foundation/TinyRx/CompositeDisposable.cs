// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SmartAddresser.Editor.Foundation.TinyRx
{
    /// <summary>
    ///     Class for handling multiple IDisposable transparently.
    /// </summary>
    internal sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;

        /// <summary>
        ///     Initialize.
        /// </summary>
        public CompositeDisposable()
        {
            _disposables = new List<IDisposable>();
        }

        /// <summary>
        ///     Initialize with capacity.
        /// </summary>
        public CompositeDisposable(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }

            _disposables = new List<IDisposable>(capacity);
        }

        /// <summary>
        ///     Whether the instance is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Count.
        /// </summary>
        public int Count => _disposables.Count;

        /// <summary>
        ///     Dispose all disposables.
        /// </summary>
        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            _disposables.Clear();
            IsDisposed = true;
        }

        /// <summary>
        ///     Add a disposable.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(IDisposable item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (IsDisposed)
            {
                item.Dispose();
                return;
            }

            _disposables.Add(item);
        }

        /// <summary>
        ///     Remove and dispose a disposable.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Remove(IDisposable item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (IsDisposed)
            {
                return false;
            }

            item.Dispose();
            _disposables.Remove(item);
            return true;
        }

        /// <summary>
        ///     Clear and dispose all disposables.
        /// </summary>
        public void Clear()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            _disposables.Clear();
        }

        public bool Contains(IDisposable item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return _disposables.Contains(item);
        }
    }
}
