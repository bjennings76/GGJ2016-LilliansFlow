using System;
using JetBrains.Annotations;
using UnityEngine;

[UsedImplicitly]
public class CameraPan : MonoBehaviour {
	[UsedImplicitly] public Vector3 StartPosition;
	[UsedImplicitly] public float Strength = 1.0f;

	[UsedImplicitly] public float HorizontalLimit = 6f;
	private float m_HorizontalLimit;
	[UsedImplicitly] public float VerticalLimit = 3f;
	private float m_VerticalLimit;

	private bool m_Initialized;

	[UsedImplicitly]
	private void OnEnable() {
		if (m_Initialized) {
			return;
		}
		m_Initialized = true;
		if (StartPosition == Vector3.zero) {
			StartPosition = transform.localPosition;
		}
	}

	[UsedImplicitly]
	private void Start() {
		OnEnable();
		m_HorizontalLimit = HorizontalLimit;
		m_VerticalLimit = VerticalLimit;
		transform.localPosition = StartPosition;
	}

	[UsedImplicitly]
	private void Update() {
		if (Math.Abs(m_HorizontalLimit - HorizontalLimit) > 0.001 || Math.Abs(m_VerticalLimit - VerticalLimit) > 0.001) {
			m_HorizontalLimit = HorizontalLimit;
			m_VerticalLimit = VerticalLimit;
		}

		Vector3 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition)*2 + new Vector3(-1, -1);
		UpdatePosition(offset);
	}

	private void UpdatePosition(Vector2 offset) {
		offset = offset*Strength;
		float h = Bound(offset.x, -1, 1)*HorizontalLimit;
		float v = Bound(offset.y, -1, 1)*VerticalLimit;
		transform.localPosition = new Vector3(StartPosition.x + h, StartPosition.y + v, StartPosition.z);
	}

	private static float Bound(float value, float min, float max) {
		return value < min ? min : (value > max ? max : value);
	}
}