using JetBrains.Annotations;
using UnityEngine;

public class Zoomer : MonoBehaviour {
	[UsedImplicitly] public float Rate = 0.1f;
	[UsedImplicitly] public float Division = 10f;
	[UsedImplicitly] public float RateModifier = 1f;

	[UsedImplicitly]
	private void Start() {
		transform.localScale = Vector3.zero;
	}

	[UsedImplicitly]
	private void Update() {
		float scale = transform.localScale.x;
		scale = scale + ((scale/Division + Rate*RateModifier)*Time.deltaTime);
		transform.localScale = new Vector3(scale, scale);
	}
}