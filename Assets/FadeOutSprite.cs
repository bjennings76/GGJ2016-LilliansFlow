using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutSprite : MonoBehaviour {
	[UsedImplicitly] public float Duration;

	private Text Text {
		get { return m_Text ? m_Text : (m_Text = GetComponent<Text>()); }
	}

	private Text m_Text;

	[UsedImplicitly]
	private void Start() {
		Text.material.DOFade(0, Duration);
	}
}