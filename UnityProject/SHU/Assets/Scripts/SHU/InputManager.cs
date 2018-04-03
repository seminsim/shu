using System.Collections.Generic;
using SHU.Sim;

namespace SHU
{
  public class InputManager
  {
    private List<IInputSource> sourceList = new List<IInputSource>();

    public void RegisterInputSource(IInputSource s)
    {
      if (!sourceList.Contains(s)) sourceList.Add(s);
    }

    public void UnregisterInputSource(IInputSource s)
    {
      if (sourceList.Contains(s)) sourceList.Remove(s);
    }

    public List<Event> GetInputs(uint tick)
    {
      var sourceListCount = sourceList.Count;
      var combinedInputs = new List<Event>();
      for (var i = 0; i < sourceListCount; ++i)
      {
        var inputs = sourceList[i].GetInputs(tick);
        combinedInputs.AddRange(inputs);
      }

      return combinedInputs;
    }
  }
}
