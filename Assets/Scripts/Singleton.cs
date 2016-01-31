using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	private static T s_Instance;
	private static bool s_Quitting;

	public virtual void OnApplicationQuit() {
		s_Instance = null;
		s_Quitting = true;
	}

	// Returns the instance of this singleton
	public static T Instance {
		get {
			Debug.AssertFormat(!s_Quitting, "An attempt was made to access {0} when the game is quitting.", typeof (T).Name);
			if (!s_Quitting) {
				if (s_Instance == null) {
					s_Instance = (T) FindObjectOfType(typeof (T));
					if (s_Instance == null) {
						GameObject container = new GameObject();
						container.name = typeof (T) + "Container";
						s_Instance = (T) container.AddComponent(typeof (T));
					}
				}
			}
			return s_Instance;
		}
	}

	public static T InstantiateFromPrefab(GameObject p) {
		GameObject go = Instantiate(p);
		if (go != null) {
			s_Instance = go.GetComponent<T>();
		}
		return s_Instance;
	}

	public static bool Exists() {
		return s_Instance != null;
	}
}