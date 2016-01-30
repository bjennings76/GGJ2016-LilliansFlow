using UnityEngine;

public static class Tools {
	public static T GetOrAddComponent<T>(this Component component) where T : Component {
		return component ? component.gameObject.GetOrAddComponent<T>() : null;
	}

	public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
		if (!go) {
			return null;
		}

		T result = go.GetComponent<T>();
		return result ? result : go.AddComponent<T>();
	}
}