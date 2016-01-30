using JetBrains.Annotations;
using UnityEngine;

public class TapPoint : MonoBehaviour {
	[UsedImplicitly] public int Order;
	private Renderer Renderer {  get { return GetComponent<Renderer>(); } }
	private Collider Collider {  get { return GetComponent<Collider>(); } }

	public void SetTap(bool enable) {
		Renderer.enabled = enable;
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
