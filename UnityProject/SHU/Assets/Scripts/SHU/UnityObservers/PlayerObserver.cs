using System;
using SHU.Sim;
using SHU.Sim.Events;
using UnityEngine;
using Event = SHU.Sim.Event;

namespace SHU.UnityObservers
{
  public class PlayerObserver : MonoBehaviour, IObserver<Event>
  {
    public Vector3 TargetPosition;

    public void OnCompleted()
    {
      Debug.Log("Target Player Object is removed. No more events will be delivered.");
    }

    public void OnError(Exception exception)
    {
      throw new NotImplementedException();
    }

    public void OnNext(Event value)
    {
      var move = value as Move;
      if (move != null)
      {
        TargetPosition = new Vector3(move.NewX, 0.0f, move.NewY);
        return;
      }
    }
  }
}