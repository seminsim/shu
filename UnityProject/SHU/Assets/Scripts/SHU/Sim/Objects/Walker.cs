using SHU.Sim.Events;

namespace SHU.Sim.Objects
{
  public class Walker : Object
  {
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
        NewX = X + 1,
        NewY = Y + 2
      });
    }
  }
}