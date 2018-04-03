using System;
using System.Diagnostics;
using SHU.FlatBuffers.Input;
using SHU.Sim.Objects;
using Sim;

namespace SHU.Sim.InputEvents
{
  public class Move : InputEvent<PlayerMovement>
  {
    public Move(PlayerMovement fbo) : base(fbo)
    {
    }

    public override void Execute(Simulation sim)
    {
      // here is implmented how this input needs to be executed in simulation
      var objectId = Fbo.ObjectId;
      var player = sim.GetObject(objectId) as Player;

      if (player == null)
      {
        Trace.TraceError("Object[{0}] is not a Player instance.", objectId);
        return;
      }

      if (Fbo.KeyUp == KeyboardAction.KeyDown) player.UpPressed = true;
      if (Fbo.KeyUp == KeyboardAction.KeyUp) player.UpPressed = false;
      if (Fbo.KeyDown == KeyboardAction.KeyDown) player.DownPressed = true;
      if (Fbo.KeyDown == KeyboardAction.KeyUp) player.DownPressed = false;
      if (Fbo.KeyLeft == KeyboardAction.KeyDown) player.LeftPressed = true;
      if (Fbo.KeyLeft == KeyboardAction.KeyUp) player.LeftPressed = false;
      if (Fbo.KeyRight == KeyboardAction.KeyDown) player.RightPressed = true;
      if (Fbo.KeyRight == KeyboardAction.KeyUp) player.RightPressed = false;
    }
  }
}
