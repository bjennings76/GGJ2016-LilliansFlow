using JetBrains.Annotations;
using UnityEngine;

public class TapPoint : MonoBehaviour {
	[UsedImplicitly] public int Order;
	private SpriteRenderer Renderer {  get { return GetComponent<SpriteRenderer>(); } }
	private Collider Collider {  get { return GetComponent<Collider>(); } }

	public void SetTap(bool enable, Sprite sprite) {
		Renderer.enabled = enable;
		Renderer.sprite = sprite;
		Collider.enabled = enable;
	}

	[UsedImplicitly]
	private void OnMouseDown() {
		Drawing drawing = GetComponentInParent<Drawing>();

		if (!drawing) {
			return;
		}

		Collider.enabled = false;

		if (drawing.TryTap(this)) {
			gameObject.AddComponent<Rotator>().DegreesPerSecond = 360;
		}
	}
}
