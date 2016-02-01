using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DrawingDirector : Singleton<DrawingDirector> {
	[UsedImplicitly] public Text ScoreDisplay;
	[UsedImplicitly] public int DrawingPoolCount = 1;
	[UsedImplicitly] public float StartNextScale = 2;
	[UsedImplicitly] public float FadeStartScale = 3;
	[UsedImplicitly] public List<DrawingInfo> DrawingList;
	[UsedImplicitly] public List<DrawingInfo> CurrentDrawingPool = new List<DrawingInfo>();

	private static Drawing s_CurrentDrawing;

	private static AudioSource s_CurrentAudioSource;
	private static List<AudioClip> s_Goods;
	private static List<AudioClip> s_Bads;
	private static List<Sprite> s_Icons;

	private static int s_LastGood;
	private static int s_LastBad;

	private int m_LastDrawingIndex = -1;

	[UsedImplicitly]
	private void Start() {
		List<string> drawingNames = Resources.LoadAll<GameObject>("Drawings").Select(go => go.name).ToList();

		if (drawingNames.Count == 0) {
			Debug.LogError("No picture paths found.", this);
			return;
		}

		s_Goods = Resources.LoadAll<AudioClip>("Audio/Good").OrderBy(a => Guid.NewGuid()).ToList();
		s_Bads = Resources.LoadAll<AudioClip>("Audio/Bad").OrderBy(a => Guid.NewGuid()).ToList();
		s_Icons = Resources.LoadAll<Sprite>("Icons").ToList();

		DrawingList = new List<DrawingInfo>(drawingNames.Select(n => new DrawingInfo("Drawings/" + n, Resources.LoadAll("Drawings/" + n), s_Icons)).OrderBy(di => Guid.NewGuid()));

		GetNewDrawings();
		UpdateScore(0.3f, false);
	}

	[UsedImplicitly]
	private void Update() {
		if (Input.GetKeyUp(KeyCode.Escape)) {
			Application.Quit();
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#endif
		}

		if (s_CurrentDrawing != null && !s_CurrentDrawing.ReadyForNext) {
			return;
		}

		DrawingInfo chosenDrawing = CurrentDrawingPool.First();
		s_CurrentDrawing = chosenDrawing.Play();
		CurrentDrawingPool.Remove(chosenDrawing);
		CurrentDrawingPool.Add(chosenDrawing);
	}

	private DrawingInfo GetNextDrawing() {
		m_LastDrawingIndex++;
		if (m_LastDrawingIndex >= DrawingList.Count) {
			m_LastDrawingIndex = 0;
			DrawingList = DrawingList.OrderBy(d => Guid.NewGuid()).ToList();
		}
		DrawingInfo drawing = DrawingList[m_LastDrawingIndex];
		drawing.State = DrawingState.Chosen;
		return drawing;
	}

	public static void PlayAudio(Component component, AudioClip clip, float volumeScale = 1) {
		if (s_CurrentAudioSource && s_CurrentAudioSource.isPlaying) {
			s_CurrentAudioSource.Stop();
		}

		AudioSource source = component.GetOrAddComponent<AudioSource>();

		Debug.Log("Playing " + clip);
		source.PlayOneShot(clip, volumeScale);
		s_CurrentAudioSource = source;
	}

	private static void PlayGood() {
		if (s_Goods.Count == 0) {
			Debug.LogError("Can't play 'good' audio. No good clips found.");
			return;
		}

		s_LastGood++;

		if (s_LastGood >= s_Goods.Count) {
			s_LastGood = 0;
			s_Goods = s_Goods.OrderBy(b => Guid.NewGuid()).ToList();
		}

		AudioClip clip = s_Goods.ElementAtOrDefault(s_LastGood);

		if (clip) {
			PlayAudio(Instance, clip);
		}
	}

	public static void PlayBad() {
		if (s_Bads.Count == 0) {
			Debug.LogError("Can't play 'bad' audio. No bad clips found.");
			return;
		}

		s_LastBad++;

		if (s_LastBad >= s_Bads.Count) {
			s_LastBad = 0;
			s_Bads = s_Bads.OrderBy(b => Guid.NewGuid()).ToList();
		}

		AudioClip clip = s_Bads.ElementAtOrDefault(s_LastBad);

		if (clip) {
			PlayAudio(Instance, clip);
		}
	}

	[UsedImplicitly]
	public static void Remove(GameObject obj) {
		if (obj == s_CurrentDrawing.gameObject) {
			s_CurrentDrawing = null;
		}

		Destroy(obj);
	}

	public static void Complete(Drawing drawing, float length) {
		Instance.CurrentDrawingPool.Remove(drawing.Info);
		Completed++;

		UpdateScore(length, true);

		if (Instance.CurrentDrawingPool.Count == 0) {
			Instance.DrawingPoolCount++;
			Instance.GetNewDrawings();
		}
	}

	private static void UpdateScore(float delay, bool playAudio) {
		if (!Instance.ScoreDisplay) {
			return;
		}
		DOTween.Sequence().PrependInterval(delay + 0.5f).OnComplete(() => {
			if (playAudio) {
				PlayGood();
			}
			Instance.ScoreDisplay.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 1);
			Instance.ScoreDisplay.text = Completed + "/" + Instance.DrawingList.Count;
		});
	}

	public static int Completed { get; set; }

	private void GetNewDrawings() {
		CurrentDrawingPool.Clear();

		while (CurrentDrawingPool.Count < DrawingPoolCount) {
			CurrentDrawingPool.Add(GetNextDrawing());
		}

		CurrentDrawingPool = CurrentDrawingPool.OrderBy(di => di.Clips.Count).ToList();
	}
}