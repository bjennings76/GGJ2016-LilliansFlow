using JetBrains.Annotations;
using UnityEngine;

public class PictureFader : MonoBehaviour {
	[UsedImplicitly] public float FadedInEnd = 0.1f;
	[UsedImplicitly] public float FadeOutStart = 30;
	[UsedImplicitly] public float FadeOutEnd = 50;

	private SpriteRenderer Renderer {
		get { return m_Renderer ? m_Renderer : (m_Renderer = GetComponent<SpriteRenderer>()); }
	}

	private SpriteRenderer m_Renderer;

	[UsedImplicitly]
	private void Update() {
		if (!Renderer) {
			return;
		}

		float alpha = GetAlpha();
		Renderer.color = new Color(1, 1, 1, alpha);
	}

	private float GetAlpha() {
		float scale = transform.localScale.x;

		if (scale < FadedInEnd) {
			return scale/FadedInEnd;
		}

		if (scale <= FadeOutStart) {
			return 1;
		}

		if (scale > FadeOutStart && scale < FadeOutEnd) {
			return (FadeOutEnd - scale)/(FadeOutEnd - FadeOutStart);
		}

		transform.localScale = Vector3.zero;
		PictureGenerator.Remove(gameObject);
		return 0;
	}
}