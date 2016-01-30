using JetBrains.Annotations;
using UnityEngine;

public class TapPoint : MonoBehaviour {
	[UsedImplicitly] public int Order;

	[UsedImplicitly]
	private void OnMouseDown() {
		Drawing drawing = GetComponentInParent<Drawing>();

		if (drawing) {
			drawing.Tap(this);
		}
	}
}
