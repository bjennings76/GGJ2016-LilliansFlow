using JetBrains.Annotations;
using UnityEngine;

public class CustomCursor : MonoBehaviour {
	[UsedImplicitly] public Texture2D CursorTexture;
	[UsedImplicitly] public CursorMode CursorMode = CursorMode.Auto;
	[UsedImplicitly] public Vector2 CursorSpot = Vector2.zero;

	[UsedImplicitly]
	private void Start() {
		Cursor.SetCursor(CursorTexture, CursorSpot, CursorMode);
	}
}