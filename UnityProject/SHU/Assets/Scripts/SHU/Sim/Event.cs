using EventId_t = System.UInt64;

namespace SHU.Sim
{
    public abstract class Event
    {
        public EventId_t Id;
        public abstract void Execute(Simulation sim);
    }
}