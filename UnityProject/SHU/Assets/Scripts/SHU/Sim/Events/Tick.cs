using System;

namespace SHU.Sim.Events
{
    public class Tick : Event
    {
        public UInt32 CurrentTick;
        public Object Obj;
        private Event _eventImplementation;

        public Tick(UInt32 tick, Object obj)
        {
            CurrentTick = tick;
            Obj = obj;
        }

        public override void Execute(Simulation sim)
        {
            // this is an exceptional event.
            // never be executed, but only used to publish tick event to observers
            throw new NotImplementedException();
        }
    }
}
