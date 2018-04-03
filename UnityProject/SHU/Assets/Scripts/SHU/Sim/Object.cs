/*
 * Object class (Object.cs)
 * All the elements in the simulation are represented as an object.
 * ex. Characters, Projectiles, Damage Areas, Explosion, and anything else
 *
 * Author: Semin Sim
 */

using System;
using System.Collections.Generic;
using SHU.Sim.Events;

using ObjectId_t = System.UInt32;

namespace SHU.Sim
{
    public abstract class Object : IObservable<Event>
    {
        public ObjectId_t Id;

        private readonly List<IObserver<Event>> _observers = new List<IObserver<Event>>();

        ~Object()
        {
            //NOTE: ranged for is not optimized in .NET 2.0
            var count = _observers.Count;
            for (var i = 0; i < count; ++i)
            {
                _observers[i].OnCompleted();
            }
            _observers.Clear();
        }

        //=====================================================
        // IObserverable<Event> implementation
        //=====================================================

        //! Subscribe
        public IDisposable Subscribe(IObserver<Event> observer)
        {
            if (!_observers.Contains(observer)) _observers.Add(observer);
            return new Unsubscriber<Event>(_observers, observer);
        }

        //=====================================================
        // Object methods
        //=====================================================

        //! Process a simulation tick of this object
        public virtual void UpdateTick(Simulation sim)
        {
            // publish tick event to observers
            PublishNext(new Tick(sim.GetCurrentTick(), this));
        }

        public void PublishNext(Event ev)
        {
            //NOTE: ranged for is not optimized in .NET 2.0
            var count = _observers.Count;
            for (var i = 0; i < count; ++i)
            {
                _observers[i].OnNext(ev);
            }
        }

        public void PublishError(Exception ex)
        {
            //NOTE: ranged for is not optimized in .NET 2.0
            var count = _observers.Count;
            for (var i = 0; i < count; ++i)
            {
                _observers[i].OnError(ex);
            }
        }
    }
}
