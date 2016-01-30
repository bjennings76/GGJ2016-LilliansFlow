using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class DrawingDirector : Singleton<DrawingDirector> {
	[UsedImplicitly] public int Count = 3;
	[UsedImplicitly] public float ScaleDelay = 3;
	[UsedImplicitly] public List<string> DrawingNames;

	[UsedImplicitly] public List<DrawingInfo> DrawingList;
	private List<DrawingInfo> m_CurrentDrawingList = new List<DrawingInfo>();

	private static AudioSource s_CurrentAudioSource;
	private static List<AudioClip> s_Goods;
	private static List<AudioClip> s_Bads;

	private static int s_LastGood;
	private static int s_LastBad;

	private int m_LastDrawing = -1;

	[UsedImplicitly]
	private void Start() {
		if (DrawingNames == null) {
			Debug.LogError("No picture paths found.", this);
			return;
		}

		DrawingList = new List<DrawingInfo>(DrawingNames.Select(n => new DrawingInfo(Resources.LoadAll("Drawings/" + n))).OrderBy(di => Guid.NewGuid()));

		s_Goods = Resources.LoadAll<AudioClip>("Audio/Good").OrderBy(a => Guid.NewGuid()).ToList();
		s_Bads = Resources.LoadAll<AudioClip>("Audio/Bad").OrderBy(a => Guid.NewGuid()).ToList();
	}

	[UsedImplicitly]
	private void Update() {
		//m_CurrentDrawingList = m_CurrentDrawingList.Where(d => d.Instance).ToList();

		if (m_CurrentDrawingList.Count >= Count) {
			return;
		}

		DrawingInfo lastDrawing = m_CurrentDrawingList.LastOrDefault();

		if (lastDrawing != null && !lastDrawing.ReadyForNext) {
			return;
		}

		int nextDrawingIndex = m_LastDrawing + 1;

		if (nextDrawingIndex >= DrawingList.Count) {
			nextDrawingIndex = 0;
		}

		DrawingInfo drawing = DrawingList[nextDrawingIndex];
		m_LastDrawing = nextDrawingIndex;
		m_CurrentDrawingList.Add(drawing);
		drawing.Init();
	}

	public static void PlayAudio(Component component, AudioClip clip, float delay = 0, float volumeScale = 1) {
		if (s_CurrentAudioSource && s_CurrentAudioSource.isPlaying && delay <= 0) {
			s_CurrentAudioSource.Stop();
		}

		AudioSource source = component.GetOrAddComponent<AudioSource>();
		//source.spatialBlend = 1;

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
		DrawingInfo drawing = Instance.m_CurrentDrawingList.FirstOrDefault(d => d.Instance == obj);

		if (drawing != null) {
			Instance.m_CurrentDrawingList.Remove(drawing);
		}
		else {
			Debug.LogError("Couldn't find " + obj + " in current drawing list.");
		}

		Destroy(obj);
	}
}