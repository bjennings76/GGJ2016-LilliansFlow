using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

/// <summary>
///   This script exits play mode whenever script compilation is detected during an editor update.
/// </summary>
[InitializeOnLoad, UsedImplicitly]
public class ExitPlayModeOnScriptCompile {
	// Static initialiser called by Unity Editor whenever scripts are loaded (editor or play mode)
	static ExitPlayModeOnScriptCompile() {
		Unused(s_Instance);
		s_Instance = new ExitPlayModeOnScriptCompile();
	}

	private ExitPlayModeOnScriptCompile() {
		EditorApplication.update += OnEditorUpdate;
	}

	~ExitPlayModeOnScriptCompile() {
		EditorApplication.update -= OnEditorUpdate;
		// Silence the unused variable warning with an if.
		s_Instance = null;
	}

	// Called each time the editor updates.
	private static void OnEditorUpdate() {
		if (!EditorApplication.isPlaying || !EditorApplication.isCompiling) {
			return;
		}

		Debug.Log("Exiting play mode due to script compilation.");
		EditorApplication.isPlaying = false;
		EditorApplication.isPlaying = true;
	}

	// Used to silence the 'is assigned by its value is never used' warning for _instance.
	private static void Unused<T>(T unusedVariable) {}

	private static ExitPlayModeOnScriptCompile s_Instance;
}