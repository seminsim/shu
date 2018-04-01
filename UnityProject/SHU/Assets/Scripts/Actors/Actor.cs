using System.Collections.Generic;
using System.Linq;
using FlatBuffers;
using UnityEngine;

namespace Actors
{
    public abstract class Actor
    {
        private List<IActorObserver> _observers;

        //! feed fake action (networked and not from server) for fake visual representation, logically meaningless
        public abstract void FeedFakeAction(uint actionId, IFlatbufferObject actionObj);
        
        //! feed action (local game or from server), logically meaningful
        public abstract void FeedAction(uint actionId, IFlatbufferObject actionObj);

        //! simulate a tick (generate game events)
        public abstract void SimulateTick();

        //! process game events
        public abstract void SimulateProcessEvent(IEvent ev);

        //! subscribe an observer for an important state change
        public void Subscribe(IActorObserver observer)
        {
            if (_observers.Contains(observer))
            {
                Debug.LogError("the observer is already subscribing this actor.");
                return;
            }
            
            _observers.Add(observer);
        }

        //! unsubscribe an observer
        public void Unsubscribe(IActorObserver observer)
        {
            _observers = _observers.Where(e => e != observer).ToList();
        }
    }
}
