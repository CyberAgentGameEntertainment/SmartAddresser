// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace SmartAddresser.Editor.Foundation.TinyRx
{
    public static class DisposableExtensions
    {
        internal static void DisposeWith(this IDisposable self, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(self);
        }
    }
}
