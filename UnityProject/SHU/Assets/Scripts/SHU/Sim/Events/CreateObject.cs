using System.Collections.Generic;

namespace SHU.Sim.Events
{
  public class CreateObject : Event
  {
    public Object Obj;
    public List<IObserver<Event>> Observers = new List<IObserver<Event>>();

    public override void Execute(Simulation sim)
    {
      sim.AddObject(Obj);
      var observerCount = Observers.Count;
      for (var i = 0; i < observerCount; ++i)
      {
        Obj.Subscribe(Observers[i]);
      }

      // let the object factories work with the new object
      sim.ObjectCreated(Obj);
    }
  }
}