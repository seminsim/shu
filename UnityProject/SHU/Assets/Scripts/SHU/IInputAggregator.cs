using System.Collections.Generic;
using SHU.Sim;

namespace SHU
{
  public interface IInputAggregator
  {
    List<Event> GetInputs(uint tick);
  }
}