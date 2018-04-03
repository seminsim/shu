using System;
using SHU.Sim.Objects;
using SHU.UnityObservers;
using UnityEngine;
using Object = SHU.Sim.Object;

namespace SHU.ObjectFactories
{
  public class UnityObjectFactory : MonoBehaviour, IObjectFactory
  {
    public GameObject RootGameObject;
    public GameObject PlayerPrefab;

    public void ObjectCreated(Sim.Simulation sim, Object obj)
    {
      var player = obj as Player;
      if (player != null)
      {
        var go = Instantiate(PlayerPrefab, RootGameObject.transform);
        go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        var observer = go.GetComponent<PlayerObserver>();
        if (observer != null)
          obj.Subscribe(observer);

        return;
      }
      throw new NotImplementedException();
    }
  }
}