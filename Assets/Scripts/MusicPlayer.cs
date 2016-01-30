using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {
	private int m_SongIndex;

	private AudioSource AudioSource {
		get { return m_AudioSource ? m_AudioSource : (m_AudioSource = this.GetOrAddComponent<AudioSource>()); }
	}
	private AudioSource m_AudioSource;

	private List<AudioClip> Songs {
		get { return m_Songs ?? (m_Songs = Resources.LoadAll<AudioClip>("Audio/Songs").OrderBy(c => Guid.NewGuid()).ToList()); }
	} 
	private List<AudioClip> m_Songs;

	[UsedImplicitly]
	void Update () {
		if (AudioSource.isPlaying) {
			return;
		}

		m_SongIndex++;

		if (m_SongIndex >= Songs.Count) {
			m_SongIndex = 0;
		}

		AudioSource.PlayOneShot(Songs[m_SongIndex]);
	}
}
