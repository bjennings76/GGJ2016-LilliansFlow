using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class DrawingInfo {
	[UsedImplicitly] public GameObject Prefab;
	[UsedImplicitly] public GameObject Instance;
	[UsedImplicitly] public List<AudioClip> Clips = new List<AudioClip>();
	[UsedImplicitly] public int Pass;

	private Drawing m_Drawing;

	public DrawingInfo(IEnumerable<Object> assets) {
		foreach (Object asset in assets) {
			GameObject prefab = asset as GameObject;

			if (prefab) {
				Prefab = prefab;
				continue;
			}

			AudioClip clip = asset as AudioClip;

			if (clip) {
				Clips.Add(clip);
				continue;
			}

			Debug.Log("Ignoring " + asset);
		}
	}

	public bool ReadyForNext {
		get { return Instance && Instance.transform.localScale.x > DrawingDirector.Instance.ScaleDelay; }
	}

	public void Init() {
		Instance = Object.Instantiate(Prefab);
		m_Drawing = Instance.GetComponent<Drawing>();
		if (m_Drawing) {
			m_Drawing.Init(this);
		}
	}
}