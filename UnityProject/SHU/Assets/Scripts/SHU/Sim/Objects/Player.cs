using System;
using SHU.Sim.Events;

namespace SHU.Sim.Objects
{
  public class Player : Object
  {
    public bool UpPressed = false;
    public bool DownPressed = false;
    public bool LeftPressed = false;
    public bool RightPressed = false;

    public int X;
    public int Y;

    public override void UpdateTick(Simulation s)
    {
      base.UpdateTick(s);  // publish tick event

      // schedule movement as an immediate event
      s.ScheduleEvent(new Move {
        ObjectId = Id,
        PreviousX = X,
        PreviousY = Y,
        NewX = X + (LeftPressed ? -1 : 0) + (RightPressed ? 1 : 0),
        NewY = Y + (DownPressed ? -1 : 0) + (UpPressed ? 1 : 0)
      });

      System.Console.WriteLine("Player Update Tick {0}", Id);
    }
  }
}