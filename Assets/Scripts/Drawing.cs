using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[UsedImplicitly]
public class Drawing : MonoBehaviour {
	private DrawingInfo m_Info;

	private List<TapPoint> TapPoints {
		get { return m_TapPoints ?? (m_TapPoints = GetComponentsInChildren<TapPoint>().OrderBy(t => t.Order).ToList()); }
	}

	private List<TapPoint> m_TapPoints;

	private int m_LastTapped;

	public void Init(DrawingInfo info) {
		m_Info = info;

		if (TapPoints.Count != m_Info.Clips.Count) {
			Debug.LogWarning("Tap points don't match audio clip count!: " + TapPoints + " taps != " + m_Info.Clips.Count + " clips.");
		}

		SetupTaps(m_Info.TapPointsVisible);
	}

	private void SetupTaps(int count) {
		for (int i = 0; i < TapPoints.Count; i++) {
			TapPoints[i].SetTap(i < count);
		}
	}

	public bool TryTap(TapPoint tapPoint) {
		int targetTap = m_LastTapped + 1;
		int currentTap = TapPoints.IndexOf(tapPoint) + 1;
		if (currentTap == targetTap) {
			GoodTap(targetTap, currentTap);
			return true;
		}
		else {
			BadTap(targetTap, currentTap);
			return false;
		}
	}

	private void GoodTap(int targetTap, int currentTap) {
		Debug.Log("Success! Tapped #" + targetTap);
		m_LastTapped = currentTap;

		AudioClip clip = m_Info.Clips.ElementAtOrDefault(targetTap - 1);

		if (clip) {
			DrawingDirector.PlayAudio(this, clip);
		}
		else {
			Debug.LogWarning("Can't find clip for tap #" + targetTap, this);
		}

		if (currentTap == m_Info.TapPointsVisible) {
			CompleteDrawing(clip ? clip.length : 0);
		}
	}

	private void CompleteDrawing(float length) {
		if (m_Info.TapPointsVisible == TapPoints.Count) {
			DrawingDirector.PlayGood(length);
			m_Info.TapPointsVisible = 1;
		}
		else {
			m_Info.TapPointsVisible++;
		}

		GetComponent<Fader>().StartFadeOut();
	}

	private void BadTap(int targetTap, int currentTap) {
		Debug.Log("Failed to tap #" + targetTap + ". Tapped #" + currentTap + " instead.");
		DrawingDirector.PlayBad();
		GetComponent<Fader>().StartFadeOut();
	}
}