using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

[UsedImplicitly]
public class Drawing : MonoBehaviour {
	public DrawingInfo Info;
	private int m_GoodTaps;

	private int TapPointsVisible {
		get { return Info.TapPointsVisible; }
	}

	private int TapPointsTotal {
		get { return Info.TotalTapPoints; }
	}

	public bool ReadyForNext {
		get { return transform.localScale.x > DrawingDirector.Instance.StartNextScale || m_ReadyForNextNow; }
	}

	private bool m_ReadyForNextNow;

	private List<TapPoint> TapPoints {
		get { return m_TapPoints ?? (m_TapPoints = GetComponentsInChildren<TapPoint>().OrderBy(t => t.Order).ToList()); }
	}

	private List<TapPoint> m_TapPoints;

	private int m_LastTapped;

	public void Play(DrawingInfo info) {
		Info = info;

		if (TapPoints.Count != Info.Clips.Count) {
			Debug.LogWarning("Tap points don't match audio clip count!: " + TapPoints + " taps != " + Info.Clips.Count + " clips.");
		}

		for (int i = 0; i < TapPoints.Count; i++) {
			TapPoints[i].SetTap(i < TapPointsVisible, info.Icons[i]);
		}
	}

	public bool TryTap(TapPoint tapPoint) {
		int targetTap = m_LastTapped + 1;
		int currentTap = TapPoints.IndexOf(tapPoint) + 1;
		if (currentTap == targetTap) {
			GoodTap(targetTap, currentTap);
			return true;
		}
		BadTap(targetTap, currentTap, tapPoint);
		return false;
	}

	private void GoodTap(int targetTap, int currentTap) {
		Debug.Log("Success! Tapped #" + targetTap);
		m_LastTapped = currentTap;
		m_GoodTaps++;

		AudioClip clip = Info.Clips.ElementAtOrDefault(targetTap - 1);

		if (clip) {
			DrawingDirector.PlayAudio(this, clip);
		}
		else {
			Debug.LogWarning("Can't find clip for tap #" + targetTap, this);
		}

		if (currentTap == TapPointsVisible) {
			CompleteDrawing(clip ? clip.length : 0);
		}
	}

	private void CompleteDrawing(float length) {
		Info.Succeed();
		if (m_GoodTaps == TapPointsTotal) {
			DrawingDirector.Complete(this, length);
			Info.Complete();
		}

		m_ReadyForNextNow = true;
		this.GetOrAddComponent<Fader>().StartFadeOut();
	}

	private void BadTap(int targetTap, int currentTap, TapPoint tapPoint) {
		Debug.Log("Failed to tap #" + targetTap + ". Tapped #" + currentTap + " instead.");
		DrawingDirector.PlayBad();
		TapPoint correctTap = TapPoints[m_GoodTaps];
		correctTap.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 1);
		m_ReadyForNextNow = true;
		this.GetOrAddComponent<Fader>().StartFadeOut();
		SpriteRenderer spriteRenderer = tapPoint.GetComponent<SpriteRenderer>();
		spriteRenderer.material.DOColor(Color.red, 1);
		Info.Fail();
	}
}