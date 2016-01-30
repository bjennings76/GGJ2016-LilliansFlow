using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class Fader : MonoBehaviour {
	[UsedImplicitly] public float FadedInEnd = 0.1f;
	[UsedImplicitly] public float FadeOutStart = 30;
	[UsedImplicitly] public float FadeOutDuration = 3;

	private bool m_FadingOut;

	private List<SpriteRenderer> Renderer {
		get { return m_Renderer ?? (m_Renderer = GetComponentsInChildren<SpriteRenderer>().ToList()); }
	}

	private List<SpriteRenderer> m_Renderer;

	[UsedImplicitly]
	private void Update() {
		float alpha = GetAlpha();
		Renderer.ForEach(r => r.color = new Color(1, 1, 1, alpha));
		ContinueFadeOut();
	}

	private float GetAlpha() {
		if (m_FadingOut) {
			return m_CurrentFadeOutDuration/FadeOutDuration;
		}

		float scale = transform.localScale.x;

		if (scale < FadedInEnd) {
			return scale/FadedInEnd;
		}

		if (scale > FadeOutStart) {
			StartFadeOut();
		}

		return 1;
	}

	private float m_CurrentFadeOutDuration;

	public void StartFadeOut() {
		if (m_FadingOut) {
			return;
		}
		Debug.Log("Starting fade out...", this);
		m_FadingOut = true;
		m_CurrentFadeOutDuration = FadeOutDuration;
	}

	private void ContinueFadeOut() {
		if (!m_FadingOut) {
			return;
		}

		m_CurrentFadeOutDuration -= Time.deltaTime;

		if (m_CurrentFadeOutDuration < 0) {
			DrawingDirector.Remove(gameObject);
			Debug.Log("Fade out complete.", this);
		}
	}
}