using JetBrains.Annotations;
using UnityEngine;

public class PictureZoomer : MonoBehaviour {
	[UsedImplicitly] public float Rate = 0.1f;
	[UsedImplicitly] public float Division = 10f;

	[UsedImplicitly]
	private void Update() {
		float scale = transform.localScale.x;
		scale = scale + ((scale/Division + Rate)*Time.deltaTime);
		transform.localScale = new Vector3(scale, scale);
	}
}