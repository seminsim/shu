using System.Collections.Generic;
using SHU.Sim;

namespace SHU
{
  public interface IInputSource
  {
    List<Event> GetInputs(uint tick);
  }
}