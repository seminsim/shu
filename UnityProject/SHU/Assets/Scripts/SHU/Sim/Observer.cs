/*
 * Observer Pattern Implementation
 * (System.IObservable is available from .NET 4.0)
 *
 * Author: Semin Sim
 */

using System;
using System.Collections.Generic;

namespace SHU.Sim
{
    public interface IObserver<in TType> {
        void OnCompleted();
        void OnError(Exception exception);
        void OnNext(TType value);
    }

    public interface IObservable<out TType> {
        IDisposable Subscribe(IObserver<TType> observer);
    }

    public class Unsubscriber<TType> : IDisposable
    {
        private readonly List<IObserver<TType>> _observers;
        private readonly IObserver<TType> _observer;

        public Unsubscriber(List<IObserver<TType>> observers, IObserver<TType> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null) _observers.Remove(_observer);
        }
    }
}
