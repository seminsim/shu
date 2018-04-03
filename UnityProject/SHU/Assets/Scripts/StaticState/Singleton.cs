using UnityEngine;

namespace SHU {
  public static class StaticComponent<T> where T : Component {
  	public static readonly T Instance;

  	static StaticComponent() {
  		GameObject go = new GameObject(typeof(T).Name);
  		Object.DontDestroyOnLoad(go);
  		Instance = go.AddComponent<T>();
  	}
  }
}