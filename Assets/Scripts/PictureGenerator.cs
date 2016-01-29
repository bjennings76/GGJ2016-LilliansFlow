using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;
using UnityEditor;

public class PictureGenerator : ScriptableSingleton<PictureGenerator> {
	[UsedImplicitly] public GameObject PictureTemplate;
	[UsedImplicitly] public int Count = 3;
	[UsedImplicitly] public float ScaleDelay = 3;

	private readonly List<GameObject> m_PictureList = new List<GameObject>();

	[UsedImplicitly]
	private void Update () {
		if (m_PictureList.Count < Count && (m_PictureList.Count == 0 || m_PictureList.Last().transform.localScale.x > ScaleDelay)) {
			m_PictureList.Add(Instantiate(PictureTemplate));
		}
	}

	[UsedImplicitly]
	public static void Remove(GameObject obj) {
		if (!instance.m_PictureList.Contains(obj)) {
			return;
		}

		instance.m_PictureList.Remove(obj);
		Destroy(obj);
	}
}