// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

namespace SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection
{
    public static class ObservableCollectionExtensions
    {
        public static IReadOnlyObservableList<TValue> ToReadOnly<TValue>(this IObservableList<TValue> self)
        {
            return (IReadOnlyObservableList<TValue>)self;
        }
    }
}
