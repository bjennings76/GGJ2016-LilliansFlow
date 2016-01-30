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
	}

	public void Tap(TapPoint tapPoint) {
		int targetTap = m_LastTapped + 1;
		int currentTap = TapPoints.IndexOf(tapPoint) + 1;
		if (currentTap == targetTap) {
			GoodTap(targetTap, currentTap);
		}
		else {
			BadTap(targetTap, currentTap);
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

		if (currentTap == TapPoints.Count) {
			CompleteDrawing(clip ? clip.length : 0);
		}
	}

	private void CompleteDrawing(float length) {
		DrawingDirector.PlayGood(length);
		Fader fader = GetComponentInParent<Fader>();
		if (fader) {
			fader.StartFadeOut();
		}
	}

	private static void BadTap(int targetTap, int currentTap) {
		Debug.Log("Failed to tap #" + targetTap + ". Tapped #" + currentTap + " instead.");
		DrawingDirector.PlayBad();
	}
}