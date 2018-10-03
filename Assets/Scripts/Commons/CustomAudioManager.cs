using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using UnityEngine;

// Application's Audio Manager, which is responsible for playing feedback
// sounds on successfully registered taps.
public class CustomAudioManager : Singleton<CustomAudioManager> {

	const float MinTimeBetweenSameClip = 0.1f;
	[SerializeField]
	private AudioSource audioSource;
	private static string lastClipName; 
	private static float lastClipTime;

	public AudioClip InputClicked;
	private float InputClickedVolume = 1f;

	public void PlayInputClicked() {
		PlayClip(InputClicked, InputClickedVolume);
	}

	private void PlayClip (AudioClip clip, float volume)
	{
		if (clip != null)
		{
			// Don't play the clip if we're spamming it
			if (clip.name == lastClipName && (Time.realtimeSinceStartup - lastClipTime) < MinTimeBetweenSameClip)
			{
				return;
			}

			lastClipName = clip.name;
			lastClipTime = Time.realtimeSinceStartup;
			if (audioSource != null)
			{
				audioSource.PlayOneShot(clip, volume);
			}
			else
			{
				AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
			}
		}
	}
}
