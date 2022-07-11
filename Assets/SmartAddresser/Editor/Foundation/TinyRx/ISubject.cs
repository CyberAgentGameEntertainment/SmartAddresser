// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace SmartAddresser.Editor.Foundation.TinyRx
{
    internal interface ISubject<T> : IObserver<T>, IObservable<T>
    {
    }
}
