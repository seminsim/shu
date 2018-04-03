using SHU.Sim;

namespace SHU
{
  public interface IObjectFactory
  {
    void ObjectCreated(Sim.Simulation sim, Object obj);
  }
}