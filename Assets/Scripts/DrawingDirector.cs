using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class DrawingDirector : Singleton<DrawingDirector> {
	[UsedImplicitly] public int DrawingPoolCount = 1;
	[UsedImplicitly] public float StartNextScale = 2;
	[UsedImplicitly] public float FadeStartScale = 3;
	[UsedImplicitly] public List<string> DrawingNames;
	[UsedImplicitly] public List<DrawingInfo> DrawingList;
	[UsedImplicitly] public List<DrawingInfo> CurrentDrawingPool = new List<DrawingInfo>();

	private static Drawing s_CurrentDrawing;

	private static AudioSource s_CurrentAudioSource;
	private static List<AudioClip> s_Goods;
	private static List<AudioClip> s_Bads;

	private static int s_LastGood;
	private static int s_LastBad;

	private int m_LastDrawingIndex = -1;

	[UsedImplicitly]
	private void Start() {
		if (DrawingNames == null) {
			Debug.LogError("No picture paths found.", this);
			return;
		}

		DrawingList = new List<DrawingInfo>(DrawingNames.Select(n => new DrawingInfo("Drawings/" + n, Resources.LoadAll("Drawings/" + n))).OrderBy(di => Guid.NewGuid()));

		s_Goods = Resources.LoadAll<AudioClip>("Audio/Good").OrderBy(a => Guid.NewGuid()).ToList();
		s_Bads = Resources.LoadAll<AudioClip>("Audio/Bad").OrderBy(a => Guid.NewGuid()).ToList();

		GetNewDrawings();
	}

	[UsedImplicitly]
	private void Update() {
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

	public static void PlayAudio(Component component, AudioClip clip, float delay = 0, float volumeScale = 1) {
		if (s_CurrentAudioSource && s_CurrentAudioSource.isPlaying && delay <= 0) {
			s_CurrentAudioSource.Stop();
		}

		AudioSource source = component.GetOrAddComponent<AudioSource>();

		Debug.Log("Playing " + clip + " with " + delay.ToString("N2") + " second delay.");
		if (delay > 0) {
			source.clip = clip;
			source.PlayScheduled(AudioSettings.dspTime + delay);
		}
		else {
			source.PlayOneShot(clip, volumeScale);
		}

		s_CurrentAudioSource = source;
	}

	public static void PlayGood(float delay = 0) {
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
			PlayAudio(Instance, clip, delay);
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
		PlayGood(length);
		Instance.CurrentDrawingPool.Remove(drawing.Info);

		if (Instance.CurrentDrawingPool.Count == 0) {
			Instance.DrawingPoolCount++;
			Instance.GetNewDrawings();
		}
	}

	private void GetNewDrawings() {
		CurrentDrawingPool.Clear();

		while (CurrentDrawingPool.Count < DrawingPoolCount) {
			CurrentDrawingPool.Add(GetNextDrawing());
		}
	}
}