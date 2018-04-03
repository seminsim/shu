using System.Collections.Generic;
using UnityEngine;
using Event = SHU.Sim.Event;

namespace SHU.InputAggregators
{
  public class LocalInputAggregator : IInputAggregator
  {
    public InputManager InputManager;

    public List<Event> GetInputs(uint tick)
    {
      return InputManager == null ? null : InputManager.GetInputs(tick);
    }
  }
}
