using JetBrains.Annotations;
using UnityEngine;

public class Rotator : MonoBehaviour {
	[UsedImplicitly] public float DegreesPerSecond = 10;
	[UsedImplicitly] public Vector3 Axis = Vector3.forward;

	[UsedImplicitly]
	private void Update() {
		transform.Rotate(Axis, DegreesPerSecond*Time.deltaTime);
	}
}