using System.Collections.Generic;
using Tick_t = System.UInt32;
using ObjectId_t = System.UInt32;
using EventId_t = System.UInt64;    // upper 32bits keep in sync with Tick_t
using EventId_Inc_t = System.UInt32;// lower 32bits
using Object = SHU.Sim.Object;

namespace SHU.Sim
{
    public class Simulation : Object
    {
        private Tick_t _tick;
        private ObjectId_t _nextObjectId = 1u;
        private EventId_Inc_t _nextEventIdInc = 1u;
        private Dictionary<ObjectId_t, Object> _objects = new Dictionary<uint, Object>();
        private SortedList<EventId_t, Event> _events = new SortedList<ulong, Event>();
        private List<IObjectFactory> _objectFactories = new List<IObjectFactory>();

        //============================================
        // Simulation public methods
        //============================================

        //! process one simulation tick
        public void UpdateTick()
        {
            // increase tick
            ++_tick;

            // consume expired events
            ConsumeEvents();

            // update tick of objects
            UpdateTick(this);    // update simulation itself
            foreach (var obj in _objects)    // update objects
            {
                obj.Value.UpdateTick(this);
            }

            // consume events (expired) - scheduled for immediate excution
            ConsumeEvents();
        }

        //! schedule a event to be excuted immediately
        public EventId_t ScheduleEvent(Event ev)
        {
            return ScheduleEvent(_tick, ev);
        }

        //! schedule a event to be executed at a target tick
        public EventId_t ScheduleEvent(Tick_t tick, Event ev)
        {
            ev.Id = GetEventId(tick);
            _events.Add(ev.Id, ev);
            return ev.Id;
        }

        //! add an object
        public ObjectId_t AddObject(Object obj)
        {
            ObjectId_t newObjectId = GetObjectId();
            obj.Id = newObjectId;
            _objects.Add(newObjectId, obj);
            return newObjectId;
        }

        public Tick_t GetCurrentTick()
        {
            return _tick;
        }

        public Object GetObject(ObjectId_t id)
        {
            return _objects[id];
        }

        public Event GetEvent(EventId_t id)
        {
            return _events[id];
        }

        public void AddObjectFactory(IObjectFactory factory)
        {
            if (!_objectFactories.Contains(factory)) _objectFactories.Add(factory);
        }

        public void RemoveObjectFactory(IObjectFactory factory)
        {
            if (_objectFactories.Contains(factory)) _objectFactories.Remove(factory);
        }

        public void ObjectCreated(Object obj)
        {
            var factoryCount = _objectFactories.Count;
            for (var i = 0; i < factoryCount; ++i)
            {
                _objectFactories[i].ObjectCreated(this, obj);
            }
        }

        //============================================
        // Simulation private methods
        //============================================

        //! consume all expired events (Simulation private)
        private void ConsumeEvents()
        {
            // consume events (expired)
            EventId_t eventIdUpperBound = (ulong)(_tick + 1) << 32;

            while (_events.Count > 0)
            {
                var ev = _events.Values[0];
                if (ev.Id >= eventIdUpperBound) break;

                ev.Execute(this);

                _events.RemoveAt(0);
            }
        }

        //! get a new event id for the target tick
        private EventId_t GetEventId(Tick_t targetTick)
        {
            return (targetTick << 32) + _nextEventIdInc++;
        }

        //! get a new object id
        private ObjectId_t GetObjectId()
        {
            return _nextObjectId++;
        }
    }
}
