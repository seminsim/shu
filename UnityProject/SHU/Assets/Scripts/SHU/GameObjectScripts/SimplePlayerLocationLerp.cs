using SHU.UnityObservers;
using UnityEngine;

namespace SHU.GameObjectScripts
{
  [RequireComponent(typeof(PlayerObserver))]
  public class SimplePlayerLocationLerp : MonoBehaviour
  {
    public float LearpingRate = 0.1f;

    private PlayerObserver _playerObserver;

    // Use this for initialization
    void Start ()
    {
      _playerObserver = GetComponent<PlayerObserver>();
    }

    // Update is called once per frame
    void Update ()
    {
      transform.localPosition = Vector3.Lerp(transform.localPosition, _playerObserver.TargetPosition, LearpingRate);
    }
  }
}
