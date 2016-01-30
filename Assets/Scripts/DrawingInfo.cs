using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class DrawingInfo {
	[UsedImplicitly] public GameObject Prefab;
	[UsedImplicitly] public string Path;
	[UsedImplicitly] public List<AudioClip> Clips = new List<AudioClip>();

	public int TotalFailures;
	public int Successes;
	public int TotalCompletions;

	public DrawingState State;
	public int TotalTapPoints { get { return Clips.Count; } }
	public int TapPointsVisible {  get { return Successes + 1; } }

	public bool Perfect {  get { return State == DrawingState.Completed && TotalFailures == 0; } }

	private Drawing m_Drawing;

	public DrawingInfo(string path, IEnumerable<Object> assets) {
		Path = path;

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

		if (!Prefab) {
			Debug.LogWarning("Couldn't find Prefab in " + Path);
		}
	}

	public Drawing Play() {
		if (!Prefab) {
			Debug.LogError("Got a bad prefab from " + Path);
			return null;
		}

		State = DrawingState.Playing;
		var instance = Object.Instantiate(Prefab);
		m_Drawing = instance.GetOrAddComponent<Drawing>();
		m_Drawing.Play(this);
		return m_Drawing;
	}

	public void Fail() {
		TotalFailures++;
		//Successes = 0;
	}

	public void Succeed() {
		Successes++;
	}

	public void Complete() {
		Successes = 0;
		TotalCompletions++;
	}
}

public enum DrawingState {
	Unseen,
	Chosen,
	Playing,
	Completed
}