using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutSprite : MonoBehaviour {
	[UsedImplicitly] public float Delay;
	[UsedImplicitly] public float Duration;

	private Text Text {
		get { return m_Text ? m_Text : (m_Text = GetComponent<Text>()); }
	}
	private Text m_Text;

	[UsedImplicitly]
	private void Update() {
		if (FadeComplete) {
			return;
		}
		while (Delay > 0) {
			Delay -= Time.deltaTime;
			return;
		}

		DOTween.ToAlpha(() => Text.color, c => Text.color = c, 0, Duration);
		FadeComplete = true;
	}

	private bool FadeComplete { get; set; }
}